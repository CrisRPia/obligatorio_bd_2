import React, { useState } from "react";
import * as backend from "@codegen/backend.api";
import { table } from "@/services/table";
import { toast } from "@/services/toast";
import { useNavigate } from "react-router-dom";

const PanelMesa: React.FC = () => {
  const navigate = useNavigate();
  const [credencial, setCredencial] = useState("");
  const [voter, setVotante] = useState<backend.FullCitizen | undefined>(
    undefined,
  );
  const [mensaje, setMensaje] = useState("");
  const [allowObserved, setAllowObserved] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleSearch = async () => {
    setMensaje("");
    setIsLoading(true);

    const result = await table.search({ credencialCivica: credencial });
    setIsLoading(false);

    // If we don't find the voter, we still set it to the result.
    // This is to override the previous voter after a search, even if it fails.
    setVotante(result);

    if (result === undefined) {
      toast("No se pudo encontrar al votante.");
    }
  };

  const handleAuthorize = async () => {
    if (!voter) return;

    const result = await table.authorize(voter);

    if (!result?.data?.success) {
      return;
    }

    setMensaje("Votante habilitado correctamente.");
    setVotante(undefined);
    setCredencial("");
  };

  const handleCloseTable = async () => {
    const result = await table.close();
    if (result?.data?.success) {
      navigate("/meas/reportes");
    }
  };

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <h2 className="text-xl font-bold mb-4">Panel de Mesa Electoral</h2>

      <div className="flex gap-2 mb-4">
        <input
          type="text"
          placeholder="Ingrese Credencial Cívica"
          value={credencial}
          onChange={(e) => setCredencial(e.target.value)}
          className="flex-1 p-2 border border-gray-300 rounded"
        />
        <button
          onClick={handleSearch}
          disabled={isLoading || credencial.trim() === ""}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
        >
          Buscar
        </button>
          <div>
            <input type="checkbox" checked={allowObserved} onChange={(e) => setAllowObserved(e.target.checked)} />
            Habilitar voto observado. Un capo el cr.
          </div>
      </div>

      {mensaje && <div className="mb-4 text-sm text-blue-700">{mensaje}</div>}

      {voter && (
        <div className="bg-white p-4 rounded shadow mb-4">
          <h3 className="text-lg font-semibold mb-2">
            Información del votante
          </h3>
          <p>
            <strong>Nombre:</strong> {voter.name} {voter.surname}
          </p>
          <p>
            <strong>Credencial:</strong> {voter.credencialCivica}
          </p>
          <p>
            <strong>Cédula:</strong> {voter.uruguayanId}
          </p>
          <p>
            <strong>Fecha de nacimiento:</strong> {voter.birthDate}
          </p>
          <button
            onClick={handleAuthorize}
            className="mt-4 bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
          >
            Habilitar para votar
          </button>
        </div>
      )}

      <button
        onClick={handleCloseTable}
        className="mt-8 text-red-600 hover:underline"
      >
        🔒 CERRAR MESA
      </button>
    </div>
  );
};

export default PanelMesa;

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
    <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4">
      <div className="container max-w-4xl mx-auto bg-white rounded-xl shadow-lg p-6 md:p-8">
        <div className="mb-8 text-center">
          <div className="mx-auto w-16 h-16 bg-[#0038A8] rounded-full flex items-center justify-center mb-4">
            {/* svg hecho por IA (si, este tambien, no se hacer svgs) */}
            <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
            </svg>
          </div>
          <h2 className="text-3xl font-bold text-gray-900 mb-4">Panel de Mesa Electoral</h2>
        </div>

        <div className="card mb-6">
          <div className="flex flex-col md:flex-row gap-4 mb-4">
            <input
              type="text"
              placeholder="Ingrese Credencial Cívica"
              value={credencial}
              onChange={(e) => setCredencial(e.target.value)}
              className="form-input flex-1"
            />
            <button
              onClick={handleSearch}
              disabled={isLoading || credencial.trim() === ""}
              className={`btn btn-primary md:w-auto ${isLoading || credencial.trim() === "" ? 'opacity-50' : ''}`}
            >
              {isLoading ? 'Buscando...' : 'Buscar'}
            </button>
          </div>

          <div className="flex items-center space-x-2 text-gray-700">
            <input
              type="checkbox"
              checked={allowObserved}
              onChange={(e) => setAllowObserved(e.target.checked)}
              className="w-4 h-4 text-[#0038A8] border-gray-300 rounded focus:ring-[#0038A8]"
            />
            <span>Habilitar voto observado</span>
          </div>
        </div>

        {mensaje && <div className="alert alert-success mb-4">{mensaje}</div>}

        {voter && (
          <div className="card mb-6">
            <h3 className="text-xl font-semibold text-gray-800 mb-4">
              Información del votante
            </h3>
            <div className="space-y-3 text-gray-700">
              <p className="flex justify-between border-b border-gray-100 pb-2">
                <strong>Nombre:</strong> 
                <span>{voter.name} {voter.surname}</span>
              </p>
              <p className="flex justify-between border-b border-gray-100 pb-2">
                <strong>Credencial:</strong> 
                <span>{voter.credencialCivica}</span>
              </p>
              <p className="flex justify-between border-b border-gray-100 pb-2">
                <strong>Cédula:</strong> 
                <span>{voter.uruguayanId}</span>
              </p>
              <p className="flex justify-between border-b border-gray-100 pb-2">
                <strong>Fecha de nacimiento:</strong> 
                <span>{voter.birthDate}</span>
              </p>
            </div>
            <button
              onClick={handleAuthorize}
              className="btn btn-primary w-full mt-6"
            >
              Habilitar para votar
            </button>
          </div>
        )}

        <div className="text-center mt-8">
          <button
            onClick={handleCloseTable}
            className="text-red-600 hover:text-red-700 font-medium text-lg transition-colors"
          >
            🔒 CERRAR MESA
          </button>
        </div>
      </div>
    </div>
  );
};

export default PanelMesa;

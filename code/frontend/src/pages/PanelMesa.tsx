import React, { useState } from 'react';
import * as backend from '@codegen/backend.api';
import { SessionStorage } from '@/services/sessionStorageService';


const PanelMesa: React.FC = () => {
  const [credencial, setCredencial] = useState('');
  const [votante, setVotante] = useState<backend.FullCitizen | null>(null);
  const [mensaje, setMensaje] = useState('');
  const [observado, setObservado] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const circuitId = 1; // simulado
  const departmentId = "DEPTO23"; // simulado

  const handleBuscar = () => {
    setMensaje('');
    setIsLoading(true);

    // Simulación de búsqueda
    setTimeout(() => {
      if (credencial.toUpperCase() === 'ABC123') {
        const dummyVotante: backend.FullCitizen = {
          birthDate: '1990-01-01',
          credencialCivica: 'ABC123',
          name: 'Juan',
          surname: 'Pérez',
          uruguayanId: 12345678,
          citizenId: "",
        };
        setVotante(dummyVotante);

        // Simulamos si es observado
        const perteneceACircuito = true; // cambiar a false para probar observado
        setObservado(!perteneceACircuito);
      } else {
        setVotante(null);
        setMensaje('No se encontró ningún votante con esa credencial.');
      }

      setIsLoading(false);
    }, 1000);
  };

  const handleAutorizar = async () => {
    if (!votante) return;

    let presidentHeaders = { headers: new Headers({
        Authorization: `Bearer ${SessionStorage.get("authToken")}`,
    }) };

    const voteId = votante.credencialCivica; // Suponemos que esto es válido
    try {
      const { status } = await backend.postTableCitizenIdAuthorize(
        votante.citizenId,
        {
          authorizeObserved: true // TODO: sacar este dato del form que llena el funcionario
        },
        presidentHeaders
      );

      if (status === 200) {
        setMensaje('Votante habilitado correctamente.');
        setVotante(null);
        setCredencial('');
      } else {
        setMensaje('Error al habilitar al votante.');
      }
    } catch (error) {
      setMensaje('Error de red al autorizar voto.');
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
          onClick={handleBuscar}
          disabled={isLoading || credencial.trim() === ''}
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
        >
          Buscar
        </button>
      </div>

      {mensaje && (
        <div className="mb-4 text-sm text-blue-700">
          {mensaje}
        </div>
      )}

      {votante && (
        <div className="bg-white p-4 rounded shadow mb-4">
          <h3 className="text-lg font-semibold mb-2">Información del votante</h3>
          <p><strong>Nombre:</strong> {votante.name} {votante.surname}</p>
          <p><strong>Credencial:</strong> {votante.credencialCivica}</p>
          <p><strong>Cédula:</strong> {votante.uruguayanId}</p>
          <p><strong>Fecha de nacimiento:</strong> {votante.birthDate}</p>
          {observado && <p className="text-yellow-600 font-semibold mt-2">⚠ Voto observado (otro circuito)</p>}

          <button
            onClick={handleAutorizar}
            className="mt-4 bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
          >
            Habilitar para votar
          </button>
        </div>
      )}

      <button className="mt-8 text-red-600 hover:underline">
        🔒 CERRAR MESA
      </button>
    </div>
  );
};

export default PanelMesa;



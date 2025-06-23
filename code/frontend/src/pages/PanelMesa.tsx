import React, { useState } from 'react';

const PanelMesa: React.FC = () => {
    const [credencial, setCredencial] = useState('');
    const [votante, setVotante] = useState<any>(null); // usar tipo real después
    const [mensaje, setMensaje] = useState('');
    const [observado, setObservado] = useState(false);

    const circuitoActual = 'SIMULADO'; // Este debería venir de la sesión

    const handleBuscar = async () => {
        setMensaje('');
        setVotante(null);
        setObservado(false);

        // Aca va el endpoint para obtener datos del votante
        const cred = credencial.toUpperCase().trim();

        if (!cred) {
            setMensaje('Debe ingresar una credencial.');
            return;
        }

        try {

            // Simulación de respuesta del backend (Voto No Observado)
            const respuestaSimulada = {
                existe: true,
                nombre: 'Juan Pérez',
                uruguayanId: '12345678',
                circuito: circuitoActual,
            };

            // Simulación de respuesta del backend (Voto Observado)
            /*
            const respuestaSimulada = {
                existe: true,
                nombre: 'Juan Pérez',
                uruguayanId: '12345678',
                circuito: 'OTRO',
            };
            */

            if (!respuestaSimulada.existe) {
                setMensaje('No se encontró un votante con esa credencial.');
                return;
            }

            setVotante(respuestaSimulada);
            if (respuestaSimulada.circuito !== circuitoActual) {
                setObservado(true);
            }
        } catch (err) {
            console.error('Error al buscar votante:', err);
            setMensaje('Error de conexión.');
        }
    };

    const handleHabilitarVoto = async () => {
        if (!votante) return;

        try {
            // 🔧 Aquí irá el endpoint real de habilitar voto
            // await putDepartmentsDepartmentIdCircuitsCircuitIdAuthorizeVoteVoteId(...)

            alert('Votante habilitado para votar.');
            setVotante(null);
            setCredencial('');
        } catch (err) {
            console.error('Error al habilitar votante:', err);
            alert('Error al habilitar votante.');
        }
    };

    const handleCerrarMesa = () => {
        // 🔧 Aquí irá el endpoint real de cierre de mesa
        alert('Mesa cerrada (simulado).');
        // Podrías redirigir a login o inicio
    };

    return (
        <div className="min-h-screen flex flex-col md:flex-row bg-gray-100">
            {/* Panel lateral */}
            <div className="w-full md:w-64 bg-white p-6 shadow-md">
                <h2 className="text-xl font-bold mb-4">Panel</h2>
                <button
                    onClick={handleCerrarMesa}
                    className="w-full bg-red-600 hover:bg-red-700 text-white font-semibold py-2 px-4 rounded"
                >
                    CERRAR MESA
                </button>
            </div>

            {/* Panel principal */}
            <div className="flex-1 p-6">
                <h2 className="text-2xl font-bold mb-4">Buscar Votante</h2>

                <div className="mb-4">
                    <input
                        type="text"
                        placeholder="Credencial Cívica"
                        value={credencial}
                        onChange={(e) => setCredencial(e.target.value)}
                        className="w-full md:w-1/2 px-4 py-3 border rounded-md focus:ring-2 focus:ring-indigo-500"
                    />
                    <button
                        onClick={handleBuscar}
                        className="ml-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 px-4 rounded"
                    >
                        Buscar
                    </button>
                </div>

                {mensaje && <p className="text-red-600">{mensaje}</p>}

                {votante && (
                    <div className="bg-white p-4 rounded shadow-md mt-4">
                        <p><strong>Nombre:</strong> {votante.nombre}</p>
                        <p><strong>Cédula:</strong> {votante.uruguayanId}</p>
                        <p><strong>Circuito:</strong> {votante.circuito}</p>

                        {observado && (
                            <p className="text-yellow-600 mt-2">
                                ⚠️ Este votante no pertenece al circuito. Voto observado.
                            </p>
                        )}

                        <button
                            onClick={handleHabilitarVoto}
                            className="mt-4 bg-green-600 hover:bg-green-700 text-white font-semibold py-2 px-4 rounded"
                        >
                            HABILITAR PARA VOTAR
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
};

export default PanelMesa;

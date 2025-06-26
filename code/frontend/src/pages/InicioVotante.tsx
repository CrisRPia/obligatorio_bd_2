import React, { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import * as backend from "@codegen/backend.api";
import { SessionStorage } from '@/services/sessionStorageService';

const InicioVotante: React.FC = () => {
    const [credencialCivica, setCredencialCivica] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
    const { circuitId } = useParams();

    const isValidCredencial = (cc: string) => /^[A-Z]{3}\d+$/.test(cc.toUpperCase());

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const cc = credencialCivica.trim().toUpperCase();

        if (!isValidCredencial(cc)) {
            setError('Credencial inválida. Formato esperado: ABC12345');
            return;
        }

        setIsLoading(true);
        setError(null);

        try {
            // Guardar token y tipo "voter"
            SessionStorage.set('userType', 'voter');
            SessionStorage.set('credencialCivica', credencialCivica);

            // Redirigir a emitir voto
            navigate(`/votar/${circuitId}`);
        } catch (err) {
            console.error('Error al validar votante:', err);
            setError('Error de conexión. Intenta de nuevo.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
            <div className="max-w-md w-full bg-white p-8 rounded-lg shadow-md">
                <h2 className="text-2xl font-bold text-center mb-6">Ingreso para Votar</h2>
                <form onSubmit={handleSubmit} className="space-y-5">
                    <div>
                        <label htmlFor="credencialCivica" className="block text-sm font-medium text-gray-700 mb-1">
                            Credencial Cívica
                        </label>
                        <input
                            type="text"
                            id="credencialCivica"
                            value={credencialCivica}
                            onChange={(e) => setCredencialCivica(e.target.value)}
                            placeholder="Ej: ABC123456"
                            className="w-full px-4 py-3 border rounded-md focus:ring-2 focus:ring-indigo-500"
                            disabled={isLoading}
                        />
                    </div>

                    {error && (
                        <p className="text-red-600 text-sm">{error}</p>
                    )}

                    <button
                        type="submit"
                        disabled={isLoading || !credencialCivica.trim()}
                        className={`w-full py-3 px-4 rounded-md text-white font-semibold transition ${isLoading
                            ? 'bg-gray-400 cursor-not-allowed'
                            : 'bg-indigo-600 hover:bg-indigo-700'
                            }`}
                    >
                        {isLoading ? 'Validando...' : 'Continuar'}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default InicioVotante;

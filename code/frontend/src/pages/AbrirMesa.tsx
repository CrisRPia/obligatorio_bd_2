import { table } from '@/services/table';
import React from 'react';
import { useNavigate } from 'react-router-dom';

const AbrirMesa: React.FC = () => {
    const navigate = useNavigate();
    const handleAbrirMesa = async () => {
        await table.open();
        await navigate('/mesa/panel');
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4">
            <div className="max-w-md w-full text-center bg-white rounded-xl shadow-lg p-6 md:p-8">
                <div className="mx-auto w-16 h-16 bg-[#0038A8] rounded-full flex items-center justify-center mb-4">
                    {/* svg hecho con IA */}
                    <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 11V7a4 4 0 118 0m-4 8v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2z" />
                    </svg>
                </div>
                <h2 className="text-2xl font-bold text-gray-900 mb-6">Apertura de Mesa</h2>
                {/*TODO: Mostrar en que circuito está*/}
                <button
                    onClick={handleAbrirMesa}
                    className="btn btn-primary w-full py-4 text-lg"
                >
                    ABRIR MESA
                </button>
            </div>
        </div>
    );
};

export default AbrirMesa;

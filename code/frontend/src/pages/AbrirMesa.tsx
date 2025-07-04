import React from 'react';
import { useNavigate } from 'react-router-dom';

const AbrirMesa: React.FC = () => {
    const navigate = useNavigate();

    const handleAbrirMesa = async () => {
        try {
            // Aca va el endpoint para abrir la mesa
            // await abrirMesa();


            navigate('/mesa/panel');
        } catch (error) {
            console.error('Error al abrir la mesa:', error);
            alert('Hubo un error al abrir la mesa. Intente nuevamente.');
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
            <div className="max-w-md w-full text-center bg-white p-8 rounded-lg shadow-lg">
                <h2 className="text-2xl font-bold mb-6">Apertura de Mesa</h2>
                {/*TODO: Mostrar en que circuito está*/}
                <button
                    onClick={handleAbrirMesa}
                    className="bg-green-600 hover:bg-green-700 text-white text-lg font-semibold py-4 px-8 rounded-lg transition"
                >
                    ABRIR MESA
                </button>
            </div>
        </div>
    );
};

export default AbrirMesa;

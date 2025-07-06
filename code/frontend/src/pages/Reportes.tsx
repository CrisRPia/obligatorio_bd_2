import React, { useState, useEffect } from 'react';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Pie } from 'react-chartjs-2';

// registramos los componentes de chart.js
ChartJS.register(ArcElement, Tooltip, Legend);

// funcion para generar un color aleatorio
const generarColorRGBA = (): string => {
    const r = Math.floor(Math.random() * 256);
    const g = Math.floor(Math.random() * 256);
    const b = Math.floor(Math.random() * 256);
    return `rgba(${r}, ${g}, ${b}, 1)`;
};

const Reportes: React.FC = () => {
    // estado para guardar los datos ya procesados (votos y color)
    const [processedData, setProcessedData] = useState<Record<string, { votos: number; color: string }>>({});

    // efecto para buscar y procesar los datos cuando el componente se monta
    useEffect(() => {
        const fetchBallotData = () => {
            //TODO: aca iria la llamada real al backend!!

            // por ahora, usamos los datos de ejemplo para simular la respuesta
            const datosDelBack = {
                'Lista Celeste y Blanca': 152,
                'Frente de Izquierda': 45,
                'Partido Liberal': 88,
                'Votos en Blanco': 25,
                'Partido Vecinal': 67,
            };

            const newData: Record<string, { votos: number; color: string }> = {};

            // transformamos los datos recibidos al formato que necesitamos
            for (const [lista, votos] of Object.entries(datosDelBack)) {
                newData[lista] = {
                    votos: votos,
                    color: generarColorRGBA(),
                };
            }
            
            // actualizamos el estado para que la vista se actualice
            setProcessedData(newData);
        };

        fetchBallotData();
    }, []);

    
    // preparamos los datos para la tabla y el grafico desde nuestro estado
    const sortedBallots = Object.entries(processedData)
        .sort(([, dataA], [, dataB]) => dataB.votos - dataA.votos);

    const chartData = {
        labels: Object.keys(processedData),
        datasets: [
            {
                label: '# de Votos',
                data: Object.values(processedData).map(d => d.votos),
                backgroundColor: Object.values(processedData).map(d => d.color.replace(', 1)', ', 0.7)')),
                borderColor: Object.values(processedData).map(d => d.color),
                borderWidth: 1,
            },
        ],
    };

    //opcines para el grafico, como la posicion de la leyenda
    const chartOptions = {
        plugins: {
            legend: {
                position: 'top' as const,
                align: 'center' as const,
            },
        },
        responsive: true,
        maintainAspectRatio: true,
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4 flex items-center justify-center">  
            <div className="container max-w-6xl mx-auto bg-white rounded-xl shadow-lg p-6 md:p-8">
                <div className="mb-8 text-center">
                    <div className="mx-auto w-16 h-16 bg-[#0038A8] rounded-full flex items-center justify-center mb-4">
                        <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                        </svg>
                    </div>
                    <h1 className="text-3xl font-bold text-gray-900 mb-4">Reportes de Mesa Electoral - Circuito Nº{}</h1>
                </div>
               
                <div className="grid md:grid-cols-2 gap-8 items-start">
                    <div className="space-y-8">
                        <div className="bg-gray-50 rounded-lg shadow-inner p-6">
                            <h2 className="text-xl font-semibold text-gray-800 mb-4">Resumen de Votación</h2>
                             <div className="space-y-3">
                                <p className="flex justify-between border-b pb-2">
                                    <span className="text-gray-600 font-bold">Total de Votos:</span>
                                    <span className="font-medium">0</span>
                                </p>
                                <p className="flex justify-between border-b pb-2">
                                    <span className="text-gray-600">Votos Regulares:</span>
                                    <span className="font-medium">0</span>
                                </p>
                                <p className="flex justify-between">
                                    <span className="text-gray-600">Votos Observados:</span>
                                    <span className="font-medium">0</span>
                                </p>
                            </div>
                        </div>

                        <div className="bg-gray-50 rounded-lg shadow-inner p-6">
                            <h2 className="text-xl font-semibold text-gray-800 mb-4">Votos por Lista</h2>
                            <div className="overflow-x-auto">
                                <table className="w-full text-left">
                                    <thead className="border-b-2 border-gray-300">
                                        <tr>
                                            <th className="py-2 px-4 font-semibold text-gray-700">Lista / Ballot</th>
                                            <th className="py-2 px-4 font-semibold text-gray-700 text-right">Votos</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {sortedBallots.map(([lista, data]) => (
                                            <tr key={lista} className="border-b border-gray-200 hover:bg-gray-100">
                                                <td className="py-3 px-4 text-gray-800">{lista}</td>
                                                <td className="py-3 px-4 font-medium text-gray-900 text-right">{data.votos}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div className="bg-gray-50 rounded-lg shadow-inner p-6 flex flex-col items-center justify-center">
                         <h2 className="text-xl font-semibold text-gray-800 mb-4">Distribución de Votos</h2>
                         <div className="w-full max-w-sm">
                            <Pie data={chartData} options={chartOptions} />
                         </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Reportes;
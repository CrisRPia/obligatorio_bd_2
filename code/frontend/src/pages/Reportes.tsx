import React from 'react';

const Reportes: React.FC = () => {
    return (
        <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4">
            <div className="container max-w-4xl mx-auto bg-white rounded-xl shadow-lg p-6 md:p-8">
                <div className="mb-8 text-center">
                    <div className="mx-auto w-16 h-16 bg-[#0038A8] rounded-full flex items-center justify-center mb-4">
                        <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                        </svg>
                    </div>
                    <h1 className="text-3xl font-bold text-gray-900 mb-4">Reportes de Mesa Electoral</h1>
                </div>

                <div className="grid md:grid-cols-2 gap-6">
                    <div className="card">
                        <h2 className="text-xl font-semibold text-gray-800 mb-4">Resumen de Votación</h2>
                        <div className="space-y-3">
                            <p className="flex justify-between border-b border-gray-100 pb-2">
                                <span className="text-gray-600">Total de Votantes:</span>
                                <span className="font-medium">0</span>
                            </p>
                            <p className="flex justify-between border-b border-gray-100 pb-2">
                                <span className="text-gray-600">Votos Emitidos:</span>
                                <span className="font-medium">0</span>
                            </p>
                            <p className="flex justify-between border-b border-gray-100 pb-2">
                                <span className="text-gray-600">Votos Pendientes:</span>
                                <span className="font-medium">0</span>
                            </p>
                        </div>
                    </div>

                    <div className="card">
                        <h2 className="text-xl font-semibold text-gray-800 mb-4">Estado de Mesa</h2>
                        <div className="space-y-3">
                            <p className="flex justify-between border-b border-gray-100 pb-2">
                                <span className="text-gray-600">Estado:</span>
                                <span className="text-green-600 font-medium">Activa</span>
                            </p>
                            <p className="flex justify-between border-b border-gray-100 pb-2">
                                <span className="text-gray-600">Hora de Apertura:</span>
                                <span className="font-medium">00:00</span>
                            </p>
                            <p className="flex justify-between border-b border-gray-100 pb-2">
                                <span className="text-gray-600">Circuito:</span>
                                <span className="font-medium">-</span>
                            </p>
                        </div>
                    </div>
                </div>

                <div className="mt-8 text-center space-x-4">
                    <button className="btn btn-primary">
                        📊 Generar Reporte
                    </button>
                    <button className="btn btn-primary">
                        💾 Guardar Datos
                    </button>
                </div>

                <div className="mt-8">
                    <h2 className="text-xl font-semibold text-gray-800 mb-4">Historial de Actividad</h2>
                    <div className="bg-gray-50 rounded-lg p-4">
                        <p className="text-gray-600 text-center">No hay actividad registrada</p>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Reportes;

import React, { useState, useEffect } from "react";
import {
    Chart as ChartJS,
    ArcElement,
    Tooltip,
    Legend,
    type ChartOptions,
    type ChartData,
} from "chart.js";
import { Pie } from "react-chartjs-2";
import { table } from "@/services/table";
import type { ElectionResult } from "@codegen/backend.api";
import { toast } from "@/services/toast";
import { boardPresidentAuth } from "@/services/auth";
import { translateElection } from "@/services/translator";


// registramos los componentes de chart.js
ChartJS.register(ArcElement, Tooltip, Legend);

// funcion para generar un color aleatorio
const generarColorRGBA = (): string => {
    const r = Math.floor(Math.random() * 256);
    const g = Math.floor(Math.random() * 256);
    const b = Math.floor(Math.random() * 256);
    return `rgba(${r}, ${g}, ${b}, 1)`;
};

type ProcessedElectionResult = {
    source: ElectionResult;
    chartData: ChartData;
    chartOptions: ChartOptions;
};

const ElectionComponent = (result: ProcessedElectionResult) => {
    return (
        <div className="grid md:grid-cols-2 gap-8 items-start mb-8">
            <div className="space-y-8">
                <div className="bg-gray-50 rounded-lg shadow-inner p-6">
                    <h2 className="text-xl font-semibold text-gray-800 mb-4">
                        {translateElection(result.source.type)}: Resumen de Votación 
                    </h2>
                    <div className="space-y-3">
                        <p className="flex justify-between border-b pb-2">
                            <span className="text-gray-600 font-bold">
                                Total de Votos:
                            </span>
                            <span className="font-medium">{result.source.totalVotes}</span>
                        </p>
                        <p className="flex justify-between border-b pb-2">
                            <span className="text-gray-600">Votos Regulares:</span>
                            <span className="font-medium">{result.source.totalVotes}</span>
                        </p>
                        <p className="flex justify-between">
                            <span className="text-gray-600">Votos Observados:</span>
                            <span className="font-medium">0</span>
                        </p>
                    </div>
                </div>
                <div className="bg-gray-50 rounded-lg shadow-inner p-6">
                    <h2 className="text-xl font-semibold text-gray-800 mb-4">
                        Votos por Lista
                    </h2>
                    <div className="overflow-x-auto">
                        <table className="w-full text-left">
                            <thead className="border-b-2 border-gray-300">
                                <tr>
                                    <th className="py-2 px-4 font-semibold text-gray-700">
                                        Lista / Ballot
                                    </th>
                                    <th className="py-2 px-4 font-semibold text-gray-700 text-right">
                                        Votos
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                {result.source.listBasedResult?.map(res => {
                                    return <tr
                                        key={res.vote.listNumber}
                                        className="border-b border-gray-200 hover:bg-gray-100"
                                    >
                                        <td className="py-3 px-4 text-gray-800">{"Lista " + res.vote.listNumber}</td>
                                        <td className="py-3 px-4 font-medium text-gray-900 text-right">
                                            {res.voteCount}
                                        </td>
                                    </tr>
                                })}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            
            <div className="bg-gray-50 rounded-lg shadow-inner p-6 flex flex-col items-center justify-center">
                <h2 className="text-xl font-semibold text-gray-800 mb-4">
                    Distribución de Votos
                </h2>
                <div className="w-full max-w-sm">
                    <Pie data={result.chartData as any} options={result.chartOptions as any} />
                </div>
            </div>
        </div>
    );
};

const Reportes: React.FC = () => {
    // estado para guardar los datos ya procesados (votos y color)
    const [processedData, setProcessedData] = useState<
    ProcessedElectionResult[] | undefined
>([]);
    const sessionData = boardPresidentAuth.getSessionData();

    // efecto para buscar y procesar los datos cuando el componente se monta
    useEffect(() => {
        const fetchBallotData = async () => {
            const results = await table.getCircuitsReport();

            if (results === undefined) {
                toast("Hubo un problema al obtener los resultados de las elecciones.");
                return;
            }

            const processed: ProcessedElectionResult[] = results.map((result) => {
                return {
                    chartData: {
                        labels: result.listBasedResult!.map((lr) => lr.vote.listNumber!),
                        datasets: [
                            {
                                label: "# de Votos",
                                data: result.listBasedResult!.map((lr) => lr.voteCount),
                                backgroundColor: result.listBasedResult!.map((_) => generarColorRGBA()),
                            },
                        ],
                    },
                    chartOptions: {
                        plugins: {
                            legend: {
                                position: "top",
                                align: "center",
                            },
                        },
                        responsive: true,
                        maintainAspectRatio: true,
                    },
                    source: result
                };
            }) ?? [];
            setProcessedData(processed);
        };

        fetchBallotData();
    }, []);

    return (
        <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4 flex items-center justify-center">
            <div className="container max-w-6xl mx-auto bg-white rounded-xl shadow-lg p-6 md:p-8">
                <div className="mb-8 text-center">
                    <div className="mx-auto w-16 h-16 bg-[#0038A8] rounded-full flex items-center justify-center mb-4">
                        <svg
                            className="w-8 h-8 text-white"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                        >
                            <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth={2}
                                d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                            />
                        </svg>
                    </div>
                    <h1 className="text-3xl font-bold text-gray-900 mb-4">
                        Reportes de Mesa Electoral - Circuito Nº{sessionData?.circuit.circuitId.circuitNumber}
                    </h1>
                </div>
                {processedData?.map(ElectionComponent)}
            </div>
        </div>
    );
};

export default Reportes;

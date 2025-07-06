import { toast } from "@/services/toast";
import { voter } from "@/services/voter";
import type { Ballot, Department, Election } from "@codegen/backend.api";
import React, { useEffect, useState } from "react";

const EmitirVoto: React.FC = () => {
    const [departments, setDepartments] = useState<Department[]>([]);
    const [elecciones, setElecciones] = useState<Election[]>([]);
    const [selecciones, setSelecciones] = useState<Map<string, Ballot | null>>(new Map());

    useEffect(() => {
        const fetchData = async () => {
            const electionsResult = await voter.getOpenElections();
            const departmentsResult = await voter.getDepartments();

            if (!electionsResult || !departmentsResult) {
                toast("Hubo un problema obteniendo los datos.");
                return;
            }

            setDepartments(departmentsResult);
            setElecciones(electionsResult.data.items);
        };

        fetchData();
    }, []);

    const handleSeleccion = (electionId: string, ballot: Ballot | null) => {
        setSelecciones(prevMap => {
            const newMap = new Map(prevMap);
            newMap.set(electionId, ballot);
            return newMap;
        });
    };

    const handleEnviar = async () => {
        if (elecciones.some(elec => selecciones.get(elec.electionId) === null)) {
            toast("Vota en todas las elecciones antes de confirmar");
            return;
        }

        const votos: Ballot[] = elecciones.map(elec => selecciones.get(elec.electionId)!);

        const result = await voter.vote({ items: votos });

        if (!result) {
            return;
        }

        toast("El voto ha sido enviado.");
    };

    // if (enviado) return (
    //     <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4">
    //         <div className="container max-w-4xl mx-auto bg-white rounded-xl shadow-lg p-6 md:p-8 text-center">
    //             <h1 className="text-3xl font-bold text-gray-900 mb-4">✅ Voto enviado correctamente</h1>
    //         </div>
    //     </div>
    // );

    return (
        <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4">
            <div className="container max-w-4xl mx-auto bg-white rounded-xl shadow-lg p-6 md:p-8">
                <h1 className="text-3xl font-bold text-gray-900 mb-8 text-center">🗳️ Emitir Voto</h1>
                {elecciones.map(elec => (
                    <div key={elec.electionId} className="card mb-6">
                        <h2 className="text-xl font-semibold text-gray-800 mb-4">{[elec.type, departments.find(d => d.departmentId === elec.departmentId)?.name].filter(Boolean).join(" - ")}</h2>
                        <ul className="space-y-3">
                            {elec.allowedBallots.map(ballot => (
                                <li key={ballot.ballotId} className="flex items-center space-x-3 p-3 hover:bg-gray-50 rounded-lg transition-colors">
                                    <label className="flex items-center cursor-pointer w-full">
                                        <input
                                            type="radio"
                                            name={elec.electionId}
                                            checked={selecciones.get(elec.electionId)?.ballotId === ballot.ballotId}
                                            onChange={() => handleSeleccion(elec.electionId, ballot)}
                                            className="w-4 h-4 text-[#0038A8] border-gray-300 focus:ring-[#0038A8]"
                                        />
                                        <span className="ml-3 text-gray-700">{"Lista " + ballot.listNumber}</span>
                                    </label>
                                </li>
                            ))}
                        </ul>
                    </div>
                ))}

                <div className="mt-8 text-center">
                    <button onClick={handleEnviar} className="btn btn-primary">
                        ✅ Confirmar Voto
                    </button>
                </div>
            </div>
        </div>
    );
}

export default EmitirVoto;

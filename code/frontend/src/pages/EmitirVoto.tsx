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

    return (
        <div>
            <h1>🗳️ Emitir Voto</h1>
            {elecciones.map(elec => (
                <div key={elec.electionId} style={{ marginBottom: "2rem" }}>
                    <h2>{[elec.type, departments.find(d => d.departmentId === elec.departmentId)?.name].filter(Boolean).join(" - ")}</h2>
                    <ul>
                        {elec.allowedBallots.map(ballot => (
                            <li key={ballot.ballotId}>
                                <label>
                                    <input
                                        type="radio"
                                        name={elec.electionId}
                                        checked={selecciones.get(elec.electionId)?.ballotId === ballot.ballotId}
                                        onChange={() => handleSeleccion(elec.electionId, ballot)}
                                    />
                                    {"Lista " + ballot.listNumber}
                                </label>
                            </li>
                        ))}
                    </ul>
                </div>
            ))}

            <button onClick={handleEnviar}>✅ Confirmar Voto</button>
        </div>
    );
}

export default EmitirVoto;

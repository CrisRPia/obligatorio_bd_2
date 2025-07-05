import { voter } from "@/services/voter";
import * as backend from "@codegen/backend.api";
import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const opcionesPorTipo: Map<string, backend.Ballot[]> = new Map();
// if (SessionStorage.get("userData")?.citizenId) {
//     const result = await backend.getElections({
//         AvailableForUser: SessionStorage.get("userData")?.citizenId,
//         OnlyOpenOrClosed: "Open",
//     })
//     for (const item of result.data.items) {
//     opcionesPorTipo.set(item.electionId, item.allowedBallots);
//     }
// }


const EmitirVoto: React.FC = () => {
    const { circuitId } = useParams<{ circuitId: string }>();
    const [elecciones, setElecciones] = useState<backend.Election[]>([]);
    const [selecciones, setSelecciones] = useState<Record<string, backend.Ballot | null>>({});
    const [enviado, setEnviado] = useState(false);

    useEffect(() => {
        const fetchElections = async () => {
            const result = await voter.getOpenElections();

            if (!result) {
                return;
            }
            const validas = result?.data.items.filter(e => opcionesPorTipo.get(e.type));
            setElecciones(validas);
        };

        fetchElections();
    }, []);

    const handleSeleccion = (electionId: string, opcion: backend.Ballot | null) => {
        setSelecciones(prev => ({ ...prev, [electionId]: opcion }));
    };

    const handleEnviar = async () => {
        if (!circuitId) return alert("Circuito no encontrado");

        if (elecciones.map(elec => selecciones[elec.electionId] !== null)) {
            throw "Whatthedogdoin";
        }

        const votos: backend.Ballot[] = elecciones.map(elec => selecciones[elec.electionId]!);

        const result = await voter.vote({ items: votos });

        if (!result) {
            return;
        }

        setEnviado(true);
    };

    if (enviado) return <p>✅ Voto enviado correctamente</p>;

    return (
        <div>
            <h1>🗳️ Emitir Voto</h1>
            {elecciones.map(elec => (
                <div key={elec.electionId} style={{ marginBottom: "2rem" }}>
                    <h2>{elec.type} - {elec.electionId}</h2>
                    <ul>
                        {(opcionesPorTipo.get(elec.type) || []).map(ballot => (
                            <li key={ballot.ballotId}>
                                <label>
                                    <input
                                        type="radio"
                                        name={elec.electionId}
                                        checked={selecciones[elec.electionId]?.electionId === ballot.electionId}
                                        onChange={() => handleSeleccion(elec.electionId, ballot)}
                                    />
                                    {"NOMBRE DE LA PAPELETA"}
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

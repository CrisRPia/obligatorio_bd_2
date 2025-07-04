import { SessionStorage } from "@/services/sessionStorageService";
import { getElections, postCitizenCitizenIdVote, type Ballot, type Ballots, type Election } from "@codegen/backend.api";
import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

// Simulación de datos
const citizenId = "12345678";

const opcionesPorTipo: Map<string, Ballot[]> = new Map();
if (SessionStorage.get("userData")?.citizenId) {
    const result = await getElections({
        AvailableForUser: SessionStorage.get("userData")?.citizenId,
        OnlyOpenOrClosed: "Open",
    })
    for (const item of result.data.items) {
    opcionesPorTipo.set(item.electionId, item.allowedBallots);
    }
}




const EmitirVoto: React.FC = () => {
    const { circuitId } = useParams<{ circuitId: string }>();
    const [elecciones, setElecciones] = useState<Election[]>([]);
    const [selecciones, setSelecciones] = useState<Record<string, Ballot | null>>({});
    const [enviado, setEnviado] = useState(false);

    useEffect(() => {
        const fetchElections = async () => {
            try {
                const { status, data } = await getElections({ OnlyOpenOrClosed: "Open" }, {});
                if (status === 200) {
                    const validas = data.items.filter((e: Election) => opcionesPorTipo.get(e.type));
                    setElecciones(validas);
                } else {
                    alert("Error al cargar elecciones");
                }
            } catch (err) {
                console.error("Error:", err);
                alert("Error al conectar con el servidor");
            }
        };

        fetchElections();
    }, []);

    const handleSeleccion = (electionId: string, opcion: Ballot | null) => {
        setSelecciones(prev => ({ ...prev, [electionId]: opcion }));
    };

    const handleEnviar = async () => {
        if (!circuitId) return alert("Circuito no encontrado");

        if (elecciones.map(elec => selecciones[elec.electionId] !== null)) {
            throw "Whatthedogdoin";
        }

        const votos: Ballot[] = elecciones.map(elec => selecciones[elec.electionId]!);

        const payload: Ballots = { items: votos };

        try {
            const { status } = await postCitizenCitizenIdVote(
                citizenId,
                payload
            );

            if (status === 200 || status === 201) {
                setEnviado(true);
            } else {
                alert("Error al enviar el voto");
            }
        } catch (err) {
            console.error("Error enviando voto:", err);
            alert("Error de red");
        }
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
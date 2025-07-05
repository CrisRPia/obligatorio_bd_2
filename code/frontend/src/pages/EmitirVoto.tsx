import { SessionStorage } from "@/services/sessionStorageService";
import { voter } from "@/services/voter";
import * as backend from "@codegen/backend.api";
import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const opcionesPorTipo: Map<string, backend.Ballot[]> = new Map();
if (SessionStorage.get("userData")?.citizenId) {
    const result = await backend.getElections({
        AvailableForUser: SessionStorage.get("userData")?.citizenId,
        OnlyOpenOrClosed: "Open",
    })
    for (const item of result.data.items) {
    opcionesPorTipo.set(item.electionId, item.allowedBallots);
    }
}

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

    if (enviado) return (
        <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4">
            <div className="container max-w-4xl mx-auto bg-white rounded-xl shadow-lg p-6 md:p-8 text-center">
                <h1 className="text-3xl font-bold text-gray-900 mb-4">✅ Voto enviado correctamente</h1>
            </div>
        </div>
    );

    return (
        <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] p-4">
            <div className="container max-w-4xl mx-auto bg-white rounded-xl shadow-lg p-6 md:p-8">
                <h1 className="text-3xl font-bold text-gray-900 mb-8 text-center">🗳️ Emitir Voto</h1>
                {elecciones.map(elec => (
                    <div key={elec.electionId} className="card mb-6">
                        <h2 className="text-xl font-semibold text-gray-800 mb-4">{elec.type} - {elec.electionId}</h2>
                        <ul className="space-y-3">
                            {(opcionesPorTipo.get(elec.type) || []).map(ballot => (
                                <li key={ballot.ballotId} className="flex items-center space-x-3 p-3 hover:bg-gray-50 rounded-lg transition-colors">
                                    <label className="flex items-center cursor-pointer w-full">
                                        <input
                                            type="radio"
                                            name={elec.electionId}
                                            checked={selecciones[elec.electionId]?.electionId === ballot.electionId}
                                            onChange={() => handleSeleccion(elec.electionId, ballot)}
                                            className="w-4 h-4 text-[#0038A8] border-gray-300 focus:ring-[#0038A8]"
                                        />
                                        <span className="ml-3 text-gray-700">{"NOMBRE DE LA PAPELETA"}</span>
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

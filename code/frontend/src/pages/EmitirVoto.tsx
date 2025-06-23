import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
  getElections,
  postCitizenCitizenIdVoteCircuitId,
  VoteType
} from "@codegen/backend.api";
import type { Election, IncomingVote, IncomingVotes } from "@codegen/backend.api.d";

// Simulación de datos
const citizenId = 12345678;

const opcionesPorTipo: Record<string, { id: string; nombre: string }[]> = {
  Presidential: [
    { id: "1", nombre: "Lista A - Partido Uno" },
    { id: "2", nombre: "Lista B - Partido Dos" }
  ],
  Plebiscite: [
    { id: "si", nombre: "Sí (Aprueba)" },
    { id: "no", nombre: "No (Rechaza)" }
  ],
  Referendum: [
    { id: "si", nombre: "Sí (Aprueba)" },
    { id: "no", nombre: "No (Rechaza)" }
  ],
  MunicipalElection: [
    { id: "3", nombre: "Lista X - Local 1" },
    { id: "4", nombre: "Lista Y - Local 2" }
  ]
};

const EmitirVoto: React.FC = () => {
  const { circuitId } = useParams<{ circuitId: string }>();
  const [elecciones, setElecciones] = useState<Election[]>([]);
  const [selecciones, setSelecciones] = useState<Record<string, string | null>>({});
  const [enviado, setEnviado] = useState(false);

  useEffect(() => {
    const fetchElections = async () => {
      try {
        const { status, data } = await getElections({ OnlyOpenOrClosed: "Open" }, {});
        if (status === 200) {
          const validas = data.items.filter((e: Election) => opcionesPorTipo[e.type]);
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

  const handleSeleccion = (electionId: string, opcion: string | null) => {
    setSelecciones(prev => ({ ...prev, [electionId]: opcion }));
  };

  const handleEnviar = async () => {
    if (!circuitId) return alert("Circuito no encontrado");

    const votos: IncomingVote[] = elecciones.map(elec => {
      const seleccion = selecciones[elec.electionId] ?? null;

      if (elec.type === "Presidential" || elec.type === "MunicipalElection") {
        return {
          electionId: elec.electionId,
          listId: seleccion ? Number(seleccion) : null,
          type: VoteType.List
        };
      } else if (elec.type === "Referendum" || elec.type === "Plebiscite") {
        return {
          electionId: elec.electionId,
          isYes: seleccion === "si" ? true : seleccion === "no" ? false : null,
          type: VoteType.Boolean
        };
      } else {
        return {
          electionId: elec.electionId,
          type: VoteType.Count // Por ahora sin implementación concreta
        };
      }
    });

    const payload: IncomingVotes = { items: votos };

    try {
      const { status } = await postCitizenCitizenIdVoteCircuitId(
        citizenId,
        circuitId,
        payload,
        {}
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
            {(opcionesPorTipo[elec.type] || []).map(op => (
              <li key={op.id}>
                <label>
                  <input
                    type="radio"
                    name={elec.electionId}
                    checked={selecciones[elec.electionId] === op.id}
                    onChange={() => handleSeleccion(elec.electionId, op.id)}
                  />
                  {op.nombre}
                </label>
              </li>
            ))}
            <li>
              <label>
                <input
                  type="radio"
                  name={elec.electionId}
                  checked={selecciones[elec.electionId] === null}
                  onChange={() => handleSeleccion(elec.electionId, null)}
                />
                Voto en Blanco / Anulado
              </label>
            </li>
          </ul>
        </div>
      ))}

      <button onClick={handleEnviar}>✅ Confirmar Voto</button>
    </div>
  );
}

export default EmitirVoto;
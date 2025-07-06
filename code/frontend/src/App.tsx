import "./App.css";
import React from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";

import Login from "./pages/Login.tsx";
import LoginVoter from "./pages/LoginVoter.tsx";
import EmitirVoto from "./pages/EmitirVoto.tsx";
import AbrirMesa from "./pages/AbrirMesa.tsx";
import PanelMesa from "./pages/PanelMesa.tsx";
import Reportes from "./pages/Reportes.tsx";
import Unauthorized from "./pages/Unauthorized.tsx";
import type { Role } from "@codegen/backend.api.ts";
import { boardPresidentAuth, voterAuth } from "./services/auth.ts";

// Hook para verificar si el usuario está autenticado y obtener su tipo de usuario
const useAuth = () => {
  const datas = [boardPresidentAuth, voterAuth].map((auth) =>
    auth.getSessionData(),
  );
  return {
    isAuthenticated: datas.some((data) => !!data?.jwtToken),
    userTypes: Array.from(new Set(datas.flatMap((data) => data?.roles))).filter(
      (role) => !!role,
    ),
  };
};

// Componente para rutas protegidas
const ProtectedRoute: React.FC<{
  children: React.ReactNode;
  allowedUserTypes?: Role[];
}> = ({ children, allowedUserTypes }) => {
  allowedUserTypes ??= [];
  const { isAuthenticated, userTypes } = useAuth();

  // Si no está autenticado, redirige al login
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // Si se especifican tipos de usuario permitidos y el usuario no coincide, redirige a no autorizado
  if (
    allowedUserTypes.length !== 0 &&
    !userTypes.some((userType) => allowedUserTypes.includes(userType!))
  ) {
    console.log("Redirecting to login.", { allowedUserTypes, userTypes });
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
};

const App: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <Router>
      <Routes>
        {/* --- Rutas para Ciudadanos Votantes --- */}
        {/* Inicio Votante */}
        <Route path="/votar/auth" element={<LoginVoter />} />

        {/* Emitir Voto */}
        <Route
          path="/votar"
          element={
            <ProtectedRoute allowedUserTypes={["Voter"]}>
              <EmitirVoto />
            </ProtectedRoute>
          }
        />

        {/* --- Rutas para Presidentes de Mesa --- */}
        {/* Ruta de Login */}
        <Route
          path="/login"
          element={
            isAuthenticated ? <Navigate to="/mesa/abrir" replace /> : <Login />
          } // Redirige al flujo de presidente si ya está autenticado
        />

        {/* Abrir Mesa */}
        <Route
          path="/mesa/abrir"
          element={
            <ProtectedRoute allowedUserTypes={["BoardPresident"]}>
              <AbrirMesa />
            </ProtectedRoute>
          }
        />

        {/* Panel de Mesa */}
        <Route
          path="/mesa/panel"
          element={
            <ProtectedRoute allowedUserTypes={["BoardPresident"]}>
              <PanelMesa />
            </ProtectedRoute>
          }
        />

        {/* Reportes */}
        <Route
          path="/mesa/reportes"
          element={
            <ProtectedRoute allowedUserTypes={["BoardPresident"]}>
              <Reportes />
            </ProtectedRoute>
          }
        />

        {/* Página de No Autorizado */}
        <Route path="/unauthorized" element={<Unauthorized />} />

        {/* 
            Ruta Raíz - redirige según autenticación 
            Nota: Esta Ruta Raíz Debemos Definir Bien Como Funcionará
        */}
        <Route
          path="/"
          element={
            isAuthenticated ? (
              <Navigate to="/votar" replace />
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        {/* Ruta Para Páginas No Encontradas (404) */}
        <Route
          path="*"
          element={
            <div className="min-h-screen bg-gray-50 flex items-center justify-center">
              <div className="text-center">
                <h1 className="text-4xl font-bold text-gray-900 mb-4">404</h1>
                <p className="text-gray-600 mb-6">Página no encontrada</p>
                <a
                  href="/"
                  className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors"
                >
                  Volver al inicio
                </a>
              </div>
            </div>
          }
        />
      </Routes>
    </Router>
  );
};

export default App;

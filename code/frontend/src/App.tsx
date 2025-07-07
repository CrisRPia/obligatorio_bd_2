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
            <ProtectedRoute allowedUserTypes={["Voter", "BoardPresident"]}>
              <EmitirVoto />
            </ProtectedRoute>
          }
        />

        {/* --- Rutas para Presidentes de Mesa --- */}
        {/* Ruta de Login */}
        <Route path="/login" element={<Login />}/>

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

        {/* Página de no autorizado */}
        <Route path="/unauthorized" element={<Unauthorized />} />

        {/* Ruta Raíz - aca te redirige segun tu autorizacion */}
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

        {/* mini pagina por si no encuentra (404) */}
        <Route
          path="*"
          element={
            <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] flex items-center justify-center p-4">
              <div className="max-w-md w-full bg-white rounded-xl shadow-lg p-6 md:p-8 text-center">
                <div className="mx-auto w-16 h-16 bg-[#0038A8] rounded-full flex items-center justify-center mb-4">
                  <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                  </svg>
                </div>
                <h1 className="text-3xl font-bold text-gray-900 mb-4">404</h1>
                <p className="text-gray-600 mb-6">Página no encontrada</p>
                <a
                  href="/"
                  className="btn btn-primary"
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

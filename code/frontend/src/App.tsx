import './App.css'
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';

import Login from './pages/Login.tsx';
import InicioVotante from './pages/InicioVotante.tsx';
import EmitirVoto from './pages/EmitirVoto.tsx';
import PanelMesa from './pages/PanelMesa.tsx';
import Reportes from './pages/Reportes.tsx';
import Unauthorized from './pages/Unauthorized.tsx';

// Hook para verificar si el usuario está autenticado y obtener su tipo de usuario
const useAuth = () => {
  const token = localStorage.getItem('authToken');
  const userType = localStorage.getItem('userType'); // 'voter', 'polling_station', 'admin'
  return { isAuthenticated: !!token, userType };
};

// Componente para rutas protegidas
const ProtectedRoute: React.FC<{
  children: React.ReactNode;
  allowedUserTypes?: string[];
}> = ({ children, allowedUserTypes }) => {
  const { isAuthenticated, userType } = useAuth();

  // Si no está autenticado, redirige al login
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // Si se especifican tipos de usuario permitidos y el usuario no coincide, redirige a no autorizado
  if (allowedUserTypes && userType && !allowedUserTypes.includes(userType)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
};

const App: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <Router>
      <Routes>
        {/* Ruta de Login */}
        <Route
          path="/login"
          element={isAuthenticated ? <Navigate to="/votar" replace /> : <Login />} // Redirige al voto si ya está autenticado
        />

        {/* --- Rutas para Ciudadanos Votantes --- */}


        {/* Inicio Votante */}
        <Route
          path="/votar/:circuitId/ingresar"
          element={
            <InicioVotante />
            /*
            <ProtectedRoute allowedUserTypes={['voter']}>
              <InicioVotante />
            </ProtectedRoute>
            */
          }
        />



        {/* Emitir Voto */}
        <Route
          path="/votar/:circuitId"
          element={
            <EmitirVoto />
            /*
            <ProtectedRoute allowedUserTypes={['voter']}>
              <EmitirVoto />
            </ProtectedRoute>
            */
          }
        />


        

        

        {/* --- Rutas para Presidentes de Mesa --- */}

        {/* Panel de Mesa */}
        <Route
          path="/mesa"
          element={
            <ProtectedRoute allowedUserTypes={['polling_station']}>
              <PanelMesa />
            </ProtectedRoute>
          }
        />

        {/* Reportes */}
        <Route
          path="/mesa/reportes"
          element={
            <ProtectedRoute allowedUserTypes={['polling_station']}>
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
            isAuthenticated ? <Navigate to="/votar" replace /> : <Navigate to="/login" replace />
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

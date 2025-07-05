import React, { useState } from 'react';
import * as backend from "@codegen/backend.api";
import { boardPresidentAuth } from '@/services/auth';

type LoginFormData = backend.LoginCredentials;

const Login: React.FC = () => {
  const [formData, setFormData] = useState<Partial<LoginFormData>>({
    credencialCivica: undefined,
    password: undefined,
    uruguayanId: undefined,
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    if (error) setError(null);
  };

  const isFormValid = (data: Partial<LoginFormData>): data is LoginFormData => {
    return data.password !== undefined && data.uruguayanId !== undefined && /^[A-Z]{3}\d+$/.test(data.credencialCivica ?? "");
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isFormValid(formData)) return;

    setIsLoading(true);
    setError(null);
    setSuccess(null);

    const response = await boardPresidentAuth.logIn(formData);

    if (response === undefined) {
      setError('Credenciales incorrectas. Intenta nuevamente.');
      return setIsLoading(false);
    }

    setSuccess('Login exitoso. Redirigiendo...');
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#E6F3FF] to-[#0038A8] flex items-center justify-center p-4">
      <div className="max-w-md w-full bg-white rounded-xl shadow-lg p-6 md:p-8">
        <div className="text-center mb-8">
          <div className="mx-auto w-16 h-16 bg-[#0038A8] rounded-full flex items-center justify-center mb-4">
            <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </div>
          <h1 className="text-2xl font-bold text-gray-900 mb-2">Acceso de Miembros de Mesa</h1>
          <p className="text-gray-600">Ingresa con tu usuario y contraseña</p>
        </div>

        <form onSubmit={handleLogin} className="space-y-6">
          <div>
            <label htmlFor="credencialCivica" className="form-label">
              Credencial Cívica
            </label>
            <input
              type="text"
              id="credencialCivica"
              name="credencialCivica"
              value={formData.credencialCivica}
              onChange={handleInputChange}
              placeholder="Ej: ABC12345"
              className="form-input"
              disabled={isLoading}
            />
          </div>

          <div>
            <label htmlFor="uruguayanId" className="form-label">
              Cédula de Identidad
            </label>
            <input
              type="number"
              id="uruguayanId"
              name="uruguayanId"
              value={formData.uruguayanId}
              onChange={handleInputChange}
              placeholder="Ej: 12345678"
              className="form-input"
              disabled={isLoading}
            />
          </div>

          <div>
            <label htmlFor="password" className="form-label">
              Contraseña
            </label>
            <input
              type="password"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleInputChange}
              placeholder="********"
              className="form-input"
              disabled={isLoading}
            />
          </div>

          {error && (
            <div className="alert alert-error">
              <div className="flex">
                <svg className="w-5 h-5 text-red-400 mr-2 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <p className="text-sm text-red-700">{error}</p>
              </div>
            </div>
          )}

          {success && (
            <div className="alert alert-success">
              <div className="flex">
                <svg className="w-5 h-5 text-green-400 mr-2 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                </svg>
                <p className="text-sm text-green-700">{success}</p>
              </div>
            </div>
          )}

          <button
            type="submit"
            disabled={isLoading || !isFormValid(formData)}
            className={`btn btn-primary w-full ${!isFormValid(formData) || isLoading ? 'opacity-50' : ''}`}
          >
            {isLoading ? (
              <div className="flex items-center justify-center">
                <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Iniciando sesión...
              </div>
            ) : (
              'Iniciar Sesión'
            )}
          </button>
        </form>

        <div className="mt-8 pt-6 border-t border-gray-200 text-center">
          <p className="text-xs text-gray-500 mb-2">
            Sistema Electoral - Elecciones Municipales 2025
          </p>
          <p className="text-xs text-gray-400">
            En caso de problemas, contacta al administrador del sistema
          </p>
        </div>
      </div>
    </div>
  );
};

export default Login;

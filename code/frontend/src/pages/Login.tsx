import React, { useState } from 'react';
import { postAuthPollingStationLogin, postAuthPoliceOfficerLogin } from '@codegen/backend.api';

interface LoginFormData {
  credencialCivica: string;
  uruguayanId: string;
}

const Login: React.FC = () => {
  const [formData, setFormData] = useState<LoginFormData>({
    credencialCivica: '',
    uruguayanId: ''
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
    // Limpiar errores cuando el usuario empiece a escribir
    if (error) setError(null);
  };

  const isFormValid = (): boolean => {
    const credencialRegex = /^[A-Z]{3}\d+$/;
    return formData.credencialCivica.trim() !== '' &&
      formData.uruguayanId.trim() !== '' &&
      credencialRegex.test(formData.credencialCivica.toUpperCase());
  };

  const tryLogin = async (loginFunction: Function, userType: string) => {
    const baseCitizen = {
      credencialCivica: formData.credencialCivica.toUpperCase(),
      uruguayanId: formData.uruguayanId
    };

    try {
      const response = await loginFunction(baseCitizen);
      if (response.status === 200) {
        // Guardar el token JWT y datos del usuario
        localStorage.setItem('authToken', response.data.jwtToken);
        localStorage.setItem('userType', userType);
        localStorage.setItem('userData', JSON.stringify(response.data.content));

        setSuccess('Login exitoso. Redirigiendo...');

        // Redirigir según el tipo de usuario
        setTimeout(() => {
          if (userType === 'polling_station') {
            window.location.href = '/mesa-electoral';
          } else if (userType === 'police_officer') {
            window.location.href = '/policia';
          }
        }, 1500);

        return true;
      }
    } catch (err) {
      console.log(`Intento de login como ${userType} falló:`, err);
    }
    return false;
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!isFormValid()) return;

    setIsLoading(true);
    setError(null);
    setSuccess(null);

    try {
      // Intentar login como miembro de mesa electoral primero
      const pollingStationSuccess = await tryLogin(postAuthPollingStationLogin, 'polling_station');
      if (pollingStationSuccess) {
        setIsLoading(false);
        return;
      }

      // Si no funciona, intentar como oficial de policía
      const policeOfficerSuccess = await tryLogin(postAuthPoliceOfficerLogin, 'police_officer');
      if (policeOfficerSuccess) {
        setIsLoading(false);
        return;
      }

      // Si ninguno funciona, mostrar error
      setError('Credenciales incorrectas. Por favor, verifica tus datos.');

    } catch (err) {
      console.error('Error en login:', err);
      setError('Error de conexión. Por favor, intenta nuevamente.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center px-4">
      <div className="max-w-md w-full bg-white rounded-xl shadow-lg p-8">
        {/* Header */}
        <div className="text-center mb-8">
          <div className="mx-auto w-16 h-16 bg-indigo-600 rounded-full flex items-center justify-center mb-4">
            <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.982 18.725A7.488 7.488 0 0012 15.75a7.488 7.488 0 00-5.982 2.975m11.963 0a9 9 0 10-11.963 0m11.963 0A8.966 8.966 0 0112 21a8.966 8.966 0 01-5.982-2.275M15 9.75a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </div>
          <h1 className="text-2xl font-bold text-gray-900 mb-2">Sistema Electoral</h1>
          <p className="text-gray-600">Ingresa tus credenciales para acceder</p>
        </div>

        {/* Selector de tipo de usuario */}
        <div className="mb-6">
          <p className="text-center text-gray-600 text-sm">
            Ingresa tus credenciales para acceder al sistema
          </p>
        </div>

        {/* Formulario */}
        <form onSubmit={handleLogin} className="space-y-6">
          <div>
            <label htmlFor="credencialCivica" className="block text-sm font-medium text-gray-700 mb-2">
              Credencial Cívica
            </label>
            <input
              type="text"
              id="credencialCivica"
              name="credencialCivica"
              value={formData.credencialCivica}
              onChange={handleInputChange}
              placeholder="Ej: ABC123456"
              className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-colors"
              disabled={isLoading}
            />
            <p className="text-xs text-gray-500 mt-1">
              Formato: 3 letras seguidas de números (ej: ABC123456)
            </p>
          </div>

          <div>
            <label htmlFor="uruguayanId" className="block text-sm font-medium text-gray-700 mb-2">
              Cédula de Identidad
            </label>
            <input
              type="text"
              id="uruguayanId"
              name="uruguayanId"
              value={formData.uruguayanId}
              onChange={handleInputChange}
              placeholder="Ej: 12345678"
              className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-colors"
              disabled={isLoading}
            />
          </div>

          {/* Mensajes de error y éxito */}
          {error && (
            <div className="bg-red-50 border border-red-200 rounded-lg p-4">
              <div className="flex">
                <svg className="w-5 h-5 text-red-400 mr-2 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <p className="text-sm text-red-700">{error}</p>
              </div>
            </div>
          )}

          {success && (
            <div className="bg-green-50 border border-green-200 rounded-lg p-4">
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
            disabled={isLoading || !isFormValid()}
            className={`w-full py-3 px-4 rounded-lg font-medium focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 transition-all ${isFormValid() && !isLoading
              ? 'bg-indigo-600 text-white hover:bg-indigo-700 cursor-pointer'
              : 'bg-gray-300 text-gray-500 cursor-not-allowed'
              }`}
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

        {/* Información adicional */}
        <div className="mt-8 pt-6 border-t border-gray-200">
          <div className="text-center">
            <p className="text-xs text-gray-500 mb-2">
              Sistema Electoral - Elecciones Municipales 2025
            </p>
            <p className="text-xs text-gray-400">
              En caso de problemas, contacta al administrador del sistema
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
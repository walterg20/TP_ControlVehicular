// walterg20
using System;
using System.Text.RegularExpressions;

namespace TP_ControlVehicular.Presentacion.Validaciones
{
    /// <summary>
    /// Clase de utilidad estática que centraliza todas las reglas de validación y expresiones regulares (RegEx) 
    /// utilizadas en los formularios de la aplicación (Usuarios, Clientes, etc.).
    /// </summary>
    public static class ValidadorGlobal
    {
        /// <summary>
        /// Valida que un texto no sea nulo, vacío ni compuesto solo por espacios, y que cumpla una longitud mínima.
        /// </summary>
        /// <param name="texto">Cadena a evaluar.</param>
        /// <param name="minLength">Longitud mínima requerida.</param>
        /// <returns>True si es válido, False en caso contrario.</returns>
        public static bool EsTextoValido(string? texto, int minLength)
        {
            return !string.IsNullOrWhiteSpace(texto) && texto.Trim().Length >= minLength;
        }

        /// <summary>
        /// Valida que un campo obligatorio contenga información (no sea nulo ni espacios en blanco).
        /// </summary>
        /// <param name="texto">Texto ingresado.</param>
        /// <returns>True si contiene texto, False si está vacío.</returns>
        public static bool EsRequerido(string? texto)
        {
            return !string.IsNullOrWhiteSpace(texto);
        }

        /// <summary>
        /// Valida el formato de DNI argentino mediante Expresión Regular.
        /// Patrón Regex: ^\d{7,10}$
        /// Explicación:
        /// - ^ : Inicio de la cadena.
        /// - \d{7,10} : Requiere entre 7 y 10 dígitos numéricos consecutivos (ej: 35123456).
        /// - $ : Fin de la cadena.
        /// </summary>
        /// <param name="dni">DNI a evaluar.</param>
        /// <returns>True si es un DNI numérico válido de 7 a 10 dígitos.</returns>
        public static bool EsDniValido(string? dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return false;
            return Regex.IsMatch(dni, @"^\d{7,10}$");
        }

        /// <summary>
        /// Valida el formato estándar de correo electrónico mediante Expresión Regular.
        /// Patrón Regex: ^[^@\s]+@[^@\s]+\.[^@\s]+$
        /// Explicación:
        /// - ^[^@\s]+ : Nombre de usuario (uno o más caracteres que NO sean '@' ni espacios en blanco).
        /// - @ : Símbolo arroba obligatorio.
        /// - [^@\s]+ : Dominio (uno o más caracteres válidos sin '@' ni espacios).
        /// - \. : Punto obligatorio separador de dominio y TLD.
        /// - [^@\s]+$ : Extensión de dominio (ej: com, ar, org) hasta el final de la cadena.
        /// </summary>
        /// <param name="email">Dirección de correo electrónico.</param>
        /// <returns>True si el formato es un email válido.</returns>
        public static bool EsEmailValido(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        /// <summary>
        /// Valida el formato de teléfono/celular argentino mediante Expresión Regular.
        /// Patrón Regex: ^\d{3,4}\s?\d{6,7}$
        /// Explicación:
        /// - ^\d{3,4} : Código de área de 3 o 4 dígitos (ej: 362 para Resistencia, 011 para CABA, 3794).
        /// - \s? : Espacio opcional entre el código de área y el número de abonado.
        /// - \d{6,7}$ : Número abonado de 6 o 7 dígitos hasta el final de la cadena (ej: 4615825).
        /// Ejemplos válidos: "3624615825", "362 4615825", "3794123456".
        /// </summary>
        /// <param name="telefono">Número de teléfono o celular.</param>
        /// <returns>True si cumple el formato numérico telefónico.</returns>
        public static bool EsTelefonoValido(string? telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return false;
            return Regex.IsMatch(telefono, @"^\d{3,4}\s?\d{6,7}$");
        }

        /// <summary>
        /// Valida que la contraseña cumpla las políticas de seguridad mínimas mediante Expresión Regular con Lookaheads.
        /// Patrón Regex: ^(?=.*[A-Z])(?=.*\d)(?=.*[#@!$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]).{6,}$
        /// Explicación detallada de cada grupo (Positive Lookaheads):
        /// - ^ : Inicio de la cadena.
        /// - (?=.*[A-Z]) : Inspección previa obligatoria: Al menos UNA letra mayúscula (A-Z).
        /// - (?=.*\d) : Inspección previa obligatoria: Al menos UN dígito numérico (0-9).
        /// - (?=.*[#@!$%^&*...]) : Inspección previa obligatoria: Al menos UN carácter especial/símbolo.
        /// - .{6,}$ : Exige que la longitud total de la contraseña sea de al menos 6 caracteres.
        /// </summary>
        /// <param name="contrasena">Contraseña a evaluar.</param>
        /// <returns>True si cumple con todas las políticas de complejidad.</returns>
        public static bool EsContrasenaValida(string? contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena)) return false;
            return Regex.IsMatch(contrasena, @"^(?=.*[A-Z])(?=.*\d)(?=.*[#@!$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{6,}$");
        }

        /// <summary>
        /// Valida la edad del usuario calculando los años cumplidos a la fecha actual.
        /// </summary>
        /// <param name="fechaNacimiento">Fecha de nacimiento seleccionada.</param>
        /// <param name="edadMinima">Edad mínima requerida (por defecto 18 años).</param>
        /// <returns>True si el usuario es mayor o igual a la edad mínima especificada.</returns>
        public static bool EsMayorDeEdad(DateTime fechaNacimiento, int edadMinima = 18)
        {
            var edad = DateTime.Today.Year - fechaNacimiento.Year;
            // Si aún no cumplió años en el año actual, restar 1 a la edad calculada
            if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;
            return edad >= edadMinima;
        }

        /// <summary>
        /// Genera una contraseña aleatoria válida que cumple obligatoriamente con todas las políticas de complejidad
        /// de EsContrasenaValida (mayúscula, minúscula, número y símbolo especial).
        /// </summary>
        /// <param name="length">Longitud deseada de la clave (mínimo 6, por defecto 10).</param>
        /// <returns>Cadena aleatoria segura que retorna True al pasar por EsContrasenaValida.</returns>
        public static string GenerarContrasenaRandom(int length = 10)
        {
            if (length < 6) length = 6;
            const string mayusculas = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string minusculas = "abcdefghijkmnopqrstuvwxyz";
            const string numeros = "23456789";
            const string especiales = "!@#$%*";

            var random = new Random();
            var chars = new List<char>
            {
                mayusculas[random.Next(mayusculas.Length)],
                minusculas[random.Next(minusculas.Length)],
                numeros[random.Next(numeros.Length)],
                especiales[random.Next(especiales.Length)]
            };

            string todos = mayusculas + minusculas + numeros + especiales;
            for (int i = chars.Count; i < length; i++)
            {
                chars.Add(todos[random.Next(todos.Length)]);
            }

            return new string(chars.OrderBy(_ => random.Next()).ToArray());
        }
    }
}

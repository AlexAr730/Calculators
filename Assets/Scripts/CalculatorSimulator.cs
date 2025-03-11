using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CalculatorSimulator : MonoBehaviour
{
    // Componentes de la calculadora
    private string contadorPrograma = "0000";  // Contador de Programa en binario (4 bits)
    private string registroDirecciones = "0000";  // Registro de Direcciones (4 bits)
    private string registroInstrucciones = "00000000";  // Registro de Instrucciones (8 bits)
    private string registroDatos = "00000000";  // Registro de Datos (8 bits)
    private string registroEntrada = "0000";  // Registro de Entrada (4 bits)
    private string acumulador = "00000000";  // Acumulador en binario (8 bits)

    // Memoria (dirección de 4 bits -> contenido de 8 bits)
    private Dictionary<string, string> memoria = new Dictionary<string, string>
    {
        { "0000", "00000100" },  // Instrucción: Sumar 4 (0000 0100)
        { "0001", "00000101" },  // Instrucción: Sumar 5 (0000 0101)
        { "0010", "01100000" },  // Instrucción: Guardar en memoria (0110 0000)
        { "0011", "01110000" },  // Instrucción: Finalizar (0111 0000)
        { "0110", "00000000" },  // Dirección para guardar en memoria (inicialmente vacía)
        { "0111", "00000000" }   // Dirección para finalizar (inicialmente vacía)
    };

    // Diccionario para mapear códigos de operación a nombres completos
    private Dictionary<string, string> codigosOperacion = new Dictionary<string, string>
    {
        { "0000", "Sumar" },
        { "0001", "Restar" },
        { "0010", "Multiplicar" },
        { "0011", "Elevar" },
        { "0100", "AND" },
        { "0101", "OR" },
        { "0110", "Guardar en memoria" },
        { "0111", "Finalizar" }
    };

    // Referencias a la UI
    public TextMeshProUGUI contadorProgramaText;
    public TextMeshProUGUI registroDireccionesText;
    public TextMeshProUGUI registroInstruccionesText;
    public TextMeshProUGUI registroDatosText;
    public TextMeshProUGUI registroEntradaText;
    public TextMeshProUGUI acumuladorText;
    public TextMeshProUGUI mensajeText;
    public TextMeshProUGUI tablaMemoriaText;  // Para mostrar la tabla de memoria
    public TextMeshProUGUI decodificadorText;  // Para mostrar la operación decodificada

    // Variables para controlar el paso actual
    private int pasoActual = 0;
    private bool programaFinalizado = false;

    // Variables para almacenar el código de operación y el valor
    private string codigoOperacion;
    private string valor;

    void Start()
    {
        ActualizarUI();
        ActualizarTablaMemoria();
    }

    // Método para avanzar paso a paso
    public void SiguientePaso()
    {
        if (programaFinalizado)
        {
            mensajeText.text = "Programa finalizado.";
            return;
        }

        switch (pasoActual)
        {
            case 0:
                // Paso 1: Transferir el contenido del Contador de programa al Registro de direcciones
                registroDirecciones = contadorPrograma;
                mensajeText.text = $"Paso 1: Registro de Direcciones cargado con la dirección {registroDirecciones}";
                break;

            case 1:
                // Paso 2: Leer la instrucción desde la memoria
                registroDatos = memoria[registroDirecciones];
                mensajeText.text = $"Paso 2: Memoria leyó el valor {registroDatos} desde la dirección {registroDirecciones}";
                break;

            case 2:
                // Paso 3: Transferir la instrucción al Registro de Instrucciones
                registroInstrucciones = registroDatos;
                mensajeText.text = $"Paso 3: Registro de Instrucciones cargado con la instrucción {registroInstrucciones}";
                break;

            case 3:
                // Paso 4: Decodificar la instrucción
                codigoOperacion = registroInstrucciones.Substring(0, 4);
                valor = registroInstrucciones.Substring(4, 4);
                string nombreOperacion = DecodificarInstruccion(codigoOperacion);
                mensajeText.text = $"Paso 4: Decodificador separó la instrucción -> Operación: {nombreOperacion}, Valor: {valor}";
                decodificadorText.text = $"Operación: {nombreOperacion}";
                break;

            case 4:
                // Paso 5: Transferir el valor al Registro de Entrada
                registroEntrada = valor;
                mensajeText.text = $"Paso 5: Registro de Entrada cargado con el valor {registroEntrada}";
                break;

            case 5:
                // Paso 6: Realizar la operación en el Acumulador
                int valorEntero = System.Convert.ToInt32(registroEntrada, 2); // Convertir el valor de entrada a decimal
                int acumuladorDecimal = System.Convert.ToInt32(acumulador, 2); // Convertir el acumulador a decimal

                if (codigoOperacion == "0000")  // Suma
                {
                    acumuladorDecimal += valorEntero; // Realizar la suma
                    acumulador = System.Convert.ToString(acumuladorDecimal, 2).PadLeft(8, '0'); // Convertir de nuevo a binario
                    mensajeText.text = $"Paso 6: Acumulador sumó {valorEntero}. Ahora tiene {acumulador}";
                }
                else if (codigoOperacion == "0110")  // Guardar en memoria
                {
                    memoria["0110"] = acumulador; // Guardar el valor del acumulador en la dirección 0110
                    mensajeText.text = $"Paso 6: Acumulador guardó el valor {acumulador} en la dirección 0110";
                    ActualizarTablaMemoria();
                }
                else if (codigoOperacion == "0111")  // Finalizar
                {
                    memoria["0111"] = acumulador; // Guardar el valor del acumulador en la dirección 0111
                    mensajeText.text = "Paso 6: Decodificador detectó la instrucción de finalizar. Fin del programa.";
                    decodificadorText.text = "Operación: Finalizar";
                    programaFinalizado = true;
                    ActualizarTablaMemoria();
                }
                break;

            case 6:
                // Paso 7: Incrementar el Contador de programa
                int contadorDecimal = System.Convert.ToInt32(contadorPrograma, 2); // Convertir binario a decimal
                contadorDecimal++; // Incrementar
                contadorPrograma = System.Convert.ToString(contadorDecimal, 2).PadLeft(4, '0'); // Convertir de nuevo a binario
                pasoActual = -1;  // Reiniciar para la siguiente instrucción
                break;
        }

        // Avanzar al siguiente paso
        pasoActual++;

        // Actualizar la UI
        ActualizarUI();
    }

    // Método para reiniciar el ciclo
    public void ReiniciarCiclo()
    {
        // Reiniciar registros y contador
        contadorPrograma = "0000";
        registroDirecciones = "0000";
        registroInstrucciones = "00000000";
        registroDatos = "00000000";
        registroEntrada = "0000";
        acumulador = "00000000";

        // Reiniciar memoria
        memoria["0110"] = "00000000"; // Reiniciar dirección de guardar en memoria
        memoria["0111"] = "00000000"; // Reiniciar dirección de finalizar

        // Reiniciar variables de control
        pasoActual = 0;
        programaFinalizado = false;

        // Actualizar la UI
        ActualizarUI();
        ActualizarTablaMemoria();
        mensajeText.text = "Ciclo reiniciado.";
    }

    // Método para decodificar la instrucción
    private string DecodificarInstruccion(string codigoOperacion)
    {
        if (codigosOperacion.ContainsKey(codigoOperacion))
        {
            return codigosOperacion[codigoOperacion];
        }
        return "Desconocido";
    }

    // Método para actualizar la interfaz de usuario
    private void ActualizarUI()
    {
        contadorProgramaText.text = $"Contador de Programa: {contadorPrograma}";
        registroDireccionesText.text = $"Registro de Direcciones: {registroDirecciones}";
        registroInstruccionesText.text = $"Registro de Instrucciones: {registroInstrucciones}";
        registroDatosText.text = $"Registro de Datos: {registroDatos}";
        registroEntradaText.text = $"Registro de Entrada: {registroEntrada}";
        acumuladorText.text = $"Acumulador: {acumulador}";
    }

    // Método para actualizar la tabla de memoria en la UI
    private void ActualizarTablaMemoria()
    {
        string tabla = "Tabla de Memoria:\n";
        tabla += "Dirección | Contenido\n";
        tabla += "-------------------\n";
        foreach (var item in memoria)
        {
            tabla += $"{item.Key}      | {item.Value}\n";
        }
        tablaMemoriaText.text = tabla;
    }


}
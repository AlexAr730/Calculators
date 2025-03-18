using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VonNeumannUI2 : MonoBehaviour
{
    //Se declaran todas las variables respectivas al UI
    [Header("Interfaz variables")]
    public TextMeshProUGUI CPUText;
    public TextMeshProUGUI MemoryText;
    public TextMeshProUGUI AccumulatorText;
    public Button NextStepButton;
    public Button ResetButton;

    //Se declaran todas las variables respectivas a las flechas
    [Header("Flechas")]
    public Image FetchArrow;
    public Image DecodeArrow;
    public Image LoadArrow;
    public Image AddArrow;
    public Image HaltArrow;
    public Image StorageArrow;

    //Se declaran todas las variables necesarias para el funcionamiento
    private int pc = 0;
    private int accumulator = 0;
    private string operation = "None";
    private int number1 = 0;
    private int number2 = 0;
    private string[] memory;
    private bool numbersLoaded = false;
    private enum ExecutionStep { Fetch, Decode, Execute }
    private ExecutionStep step = ExecutionStep.Fetch;
    private string currentInstruction = "";
    private ExecutionStep previousStep = ExecutionStep.Fetch;
    private string InputNumber1;
    private string InputNumber2;

    //Carga las funciones y los botones
    void Start()
    {
        ResetExecution();
        NextStepButton.onClick.AddListener(AdvanceStep);
        ResetButton.onClick.AddListener(ResetExecution);
    }

    //Carga los numero mandados desde la calculadora
    void Update()
    {
        Control control = GetComponent<Control>();
        InputNumber1 = control.numero1;
        InputNumber2 = control.numero2;   
    }

    //Funcion que se usa con el boton de siguiente, tras presioanrlo pasa al siguiente paso
    void AdvanceStep()
    {
        //Lee los numeros que se mandan en la calculadora
        if (!numbersLoaded)
        {
            ReadNumbers();
            return;
        }

        if (pc >= memory.Length) return; // Detener si ya termin�

        // Guardar el paso actual antes de cambiarlo
        previousStep = step;

        //Aqui se encuentran los estados y la operacion correspondiente
        switch (step)
        {
            case ExecutionStep.Fetch:
                currentInstruction = memory[pc]; // Obtener instrucci�n
                operation = "FETCH";//se define la operacion
                step = ExecutionStep.Decode;//Guarda el siguiente paso
                break;

            case ExecutionStep.Decode:
                operation = $"DECODE ({currentInstruction})"; // Decodificar instrucci�n
                step = ExecutionStep.Execute;
                break;

            case ExecutionStep.Execute:
                ExecuteInstruction(currentInstruction); // Ejecutar
                step = ExecutionStep.Fetch;
                pc++; // Mover al siguiente solo despu�s de ejecutar
                break;
        }

        UpdateUI();//Actualiza la Ui luego de que ocurre el paso
        UpdateArrows();//Actualiza las flechas luego de que ocurre el paso
    }

    //Funcion que actualiza las UI de las flechas
    void UpdateArrows()
    {
        // Desactivar todas las flechas
        FetchArrow.enabled = false;
        DecodeArrow.enabled = false;
        LoadArrow.enabled = false;
        AddArrow.enabled = false;
        HaltArrow.enabled = false;
        StorageArrow.enabled = false;

        // Activar la flecha seg�n el paso ANTERIOR
        switch (previousStep)
        {
            case ExecutionStep.Fetch:
                FetchArrow.enabled = true;
                break;

            case ExecutionStep.Decode:
                DecodeArrow.enabled = true;
                break;

            case ExecutionStep.Execute:
                // Activar solo la flecha correspondiente a la instrucci�n anterior
                switch (currentInstruction)
                {
                    case "LOAD R1":
                    case "LOAD R2":
                        FetchArrow.enabled = true;
                        LoadArrow.enabled = true;
                        AddArrow.enabled = true;
                        break;
                    case "ADD R1, R2":
                        LoadArrow.enabled = true;
                        AddArrow.enabled = true;
                        break;
                    case "STORE ACC":
                        StorageArrow.enabled = true;
                        AddArrow.enabled = true;
                        break;
                    case "HALT":
                        HaltArrow.enabled = true;
                        break;
                }
                break;
        }
    }

    //Funcion que define los pasos del sistema 
    void ExecuteInstruction(string instruction)
    {
        //Switch que contiene todos los pasos y almacena el sisguiente
        switch (instruction)
        {
            case "LOAD R1":
                accumulator = number1;
                operation = "EXECUTE LOAD R1";
                break;
            case "LOAD R2":
                accumulator = number2;
                operation = "EXECUTE LOAD R2";
                break;
            case "ADD R1, R2":
                accumulator = number1 + number2;
                operation = "EXECUTE ADD";
                break;
            case "STORE ACC":
                memory[3] = "RESULT = " + accumulator;
                operation = "EXECUTE STORE";
                break;
            case "HALT":
                operation = "EXECUTE STOP";
                break;
        }
    }

    //Se leen los numeros insertados en el field y se carga memoria
    void ReadNumbers()
    {

        if (int.TryParse(InputNumber1, out number1) && int.TryParse(InputNumber2, out number2))
        {
            //Memoria usada durante la ejecucion del programa
            memory = new string[]
            {
                "LOAD R1",
                "LOAD R2",
                "ADD R1, R2",
                "STORE ACC",
                "HALT"
            };
            numbersLoaded = true;
            UpdateUI();
        }
        else
        {
            Debug.Log("Por favor, ingresa nomeros validos.");
        }
    }
    //Funcion que se llama al presionar el boton de reiniciar, retornando todas las variables a 0 o su estado inicial
    void ResetExecution()
    {
        pc = 0;
        accumulator = 0;
        operation = "None";
        memory = new string[5] { "?", "?", "?", "?", "?" };
        InputNumber1 = "";
        InputNumber2 = "";
        numbersLoaded = false;
        UpdateUI();
    }
    //Funcion usada para actualizar el UI
    void UpdateUI()
    {
        CPUText.text = $"CPU\nContador: {pc}\nOperacion: {operation}";
        MemoryText.text = "Memoria:\n" + string.Join("\n", memory);
        AccumulatorText.text = $"{accumulator}";
    }
}
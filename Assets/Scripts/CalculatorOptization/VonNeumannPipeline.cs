using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VonNeumannPipeline : MonoBehaviour
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
    private int pc = 0; // Contador de programa
    private int accumulator = 0; // Acumulador
    private string[] memory; // Memoria de instrucciones
    private bool numbersLoaded = false; // Indica si los n�meros se han cargado
    private int[][] inputPairs; // Pares de n�meros para las operaciones
    private string InputNumber1;
    private string InputNumber2;
    private enum ExecutionStep { Fetch, Decode, Execute, Done }
    private ExecutionStep[] pipelineSteps; // Estado de cada instrucci�n en el pipeline
    private string[] pipelineInstructions; // Instrucciones en el pipeline
    private int cycle = 0; // Ciclo actual del pipeline
    private bool isHalted = false; // Indica si el programa ha terminado

    //Carga las funciones y los botones
    void Start()
    {
        ResetExecution();
        NextStepButton.onClick.AddListener(AdvancePipeline);
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
    void AdvancePipeline()
    {
        if (isHalted) // Si el programa ha terminado, no avanzar
        {
            Debug.Log("El programa ha terminado. Presiona Reset para reiniciar.");
            return;
        }
        //Lee los numeros que se mandan en la calculadora
        if (!numbersLoaded)
        {
            ReadNumbers();
            return;
        }

        // Avanzar todas las instrucciones en el pipeline
        for (int i = pipelineSteps.Length - 1; i >= 0; i--)
        {
            if (pipelineSteps[i] != ExecutionStep.Done)
            {
                pipelineSteps[i]++;

                if (pipelineSteps[i] == ExecutionStep.Execute)
                {
                    ExecuteInstruction(pipelineInstructions[i], i);
                }
            }
        }

        // Si hay m�s instrucciones, agregar una nueva al pipeline
        if (pc < memory.Length)
        {
            for (int i = 0; i < pipelineSteps.Length - 1; i++)
            {
                pipelineSteps[i] = pipelineSteps[i + 1];
                pipelineInstructions[i] = pipelineInstructions[i + 1];
            }

            pipelineSteps[pipelineSteps.Length - 1] = ExecutionStep.Fetch;
            pipelineInstructions[pipelineSteps.Length - 1] = memory[pc];
            pc++;
        }

        cycle++;//Aumenta los ciclos por siguiente
        UpdateUI();//Actualiza la Ui luego de que ocurre el paso
        UpdateArrows();//Actualiza las flechas luego de que ocurre el paso
    }

    //Funcion que define los pasos del sistema 
    void ExecuteInstruction(string instruction, int index)
    {
        switch (instruction)
        {
            case "LOAD R1":
                accumulator = inputPairs[0][0]; // Cargar el primer n�mero del primer par
                break;
            case "LOAD R2":
                accumulator = inputPairs[0][1]; // Cargar el segundo n�mero del primer par
                break;
            case "ADD R1, R2":
                accumulator = inputPairs[0][0] + inputPairs[0][1]; // Sumar los n�meros del primer par
                break;
            case "LOAD R3":
                accumulator = inputPairs[1][0]; // Cargar el primer n�mero del segundo par
                break;
            case "LOAD R4":
                accumulator = inputPairs[1][1]; // Cargar el segundo n�mero del segundo par
                break;
            case "ADD R3, R4":
                accumulator = inputPairs[1][0] + inputPairs[1][1]; // Sumar los n�meros del segundo par
                break;
            case "STORE ACC":
                if (accumulator == inputPairs[0][0] + inputPairs[0][1]) // Primera suma (primer par)
                {
                    memory[3] = "RESULT1= " + accumulator; // Guardar el resultado del primer par
                }
                else if (accumulator == inputPairs[1][0] + inputPairs[1][1]) // Segunda suma (segundo par)
                {
                    memory[7] = "RESULT2= " + accumulator; // Guardar el resultado del segundo par
                }
                break;
            case "HALT":
                isHalted = true; // Marcar que el programa ha terminado
                Debug.Log("Ejecucion finalizada.");
                break;
        }
    }

    //Se leen los numeros insertados en el field y se carga memoria
    void ReadNumbers()
    {
        if (int.TryParse(InputNumber1, out int n1) && int.TryParse(InputNumber2, out int n2))
        {
            //Se toma R3 como el primer numero +1 y R4 el segundo numero +1
            inputPairs = new int[2][]
            {
                new int[] { n1, n2 },         // Primer par
                new int[] { n1 + 1, n2 + 1 }  // Segundo par
            };
            //Memoria usada durante la ejecucion del programa
            memory = new string[]
            {
                "LOAD R1",
                "LOAD R2",
                "ADD R1, R2",
                "STORE ACC",
                "LOAD R3",
                "LOAD R4",
                "ADD R3, R4",
                "STORE ACC",
                "HALT"
            };

            pipelineSteps = new ExecutionStep[3]; // Pipeline de 3 etapas
            pipelineInstructions = new string[3]; // Instrucciones en el pipeline
            numbersLoaded = true;
            UpdateUI();//Actualiza el UI
        }
        else
        {
            Debug.Log("Por favor, ingresa numeros validos.");
        }
    }
    //Funcion que se llama al presionar el boton de reiniciar, retornando todas las variables a 0 o su estado inicial
    void ResetExecution()
    {
        pc = 0;
        accumulator = 0;
        memory = new string[]
        {
            "LOAD R1",
            "LOAD R2",
            "ADD R1, R2",
            "STORE ACC",
            "LOAD R3",
            "LOAD R4",
            "ADD R3, R4",
            "STORE ACC",
            "HALT"
        };
        inputPairs = new int[2][];
        numbersLoaded = false;
        pipelineSteps = new ExecutionStep[3];
        pipelineInstructions = new string[3];
        cycle = 0;
        isHalted = false; // Reiniciar el estado de HALT
        InputNumber1 = "";
        InputNumber2 = "";
        UpdateUI();
    }
    //Funcion usada para actualizar el UI
    void UpdateUI()
    {
        CPUText.text = $"CPU\nCiclo: {cycle}";
        MemoryText.text =  string.Join("\n", memory);
        AccumulatorText.text = $"{accumulator}";
    }
    //Funcion que actualiza las UI de las flechas
    void UpdateArrows()
    {
        //Se inicializan las flechas en false
        FetchArrow.enabled = false;
        DecodeArrow.enabled = false;
        LoadArrow.enabled = false;
        AddArrow.enabled = false;
        HaltArrow.enabled = false;
        StorageArrow.enabled = false;

        for (int i = 0; i < pipelineSteps.Length; i++)
        {
            switch (pipelineSteps[i])
            {
                case ExecutionStep.Fetch:
                    FetchArrow.enabled = true;
                    break;
                case ExecutionStep.Decode:
                    DecodeArrow.enabled = true;
                    break;
                case ExecutionStep.Execute:
                    switch (pipelineInstructions[i])
                    {
                        case "LOAD R1":
                        case "LOAD R3":
                            FetchArrow.enabled = true;
                            LoadArrow.enabled = true;
                            AddArrow.enabled = true;
                            break;
                        case "ADD R1, R2":
                        case "ADD R3, R4":
                            LoadArrow.enabled = true;
                            AddArrow.enabled = true;
                            break;
                        case "STORE ACC":
                            StorageArrow.enabled = true; // Activar la flecha de STORE
                            break;
                        case "HALT":
                            HaltArrow.enabled = true;
                            break;
                    }
                    break;
            }
        }
    }
}
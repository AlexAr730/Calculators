using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VonNeumannPipeline : MonoBehaviour
{
    public TextMeshProUGUI CPUText;
    public TextMeshProUGUI MemoryText;
    public TextMeshProUGUI AccumulatorText;
    public TMP_InputField InputNumber1;
    public TMP_InputField InputNumber2;
    public Button NextStepButton;
    public Button ResetButton;

    public Image FetchArrow;
    public Image DecodeArrow;
    public Image LoadArrow;
    public Image AddArrow;
    public Image HaltArrow;

    private int pc = 0; // Contador de programa
    private int accumulator = 0; // Acumulador
    private string[] memory; // Memoria de instrucciones
    private bool numbersLoaded = false; // Indica si los números se han cargado
    private int[][] inputPairs; // Pares de números para las operaciones

    private enum ExecutionStep { Fetch, Decode, Execute, Done }
    private ExecutionStep[] pipelineSteps; // Estado de cada instrucción en el pipeline
    private string[] pipelineInstructions; // Instrucciones en el pipeline
    private int cycle = 0; // Ciclo actual del pipeline
    private bool isHalted = false; // Indica si el programa ha terminado

    void Start()
    {
        ResetExecution();
        NextStepButton.onClick.AddListener(AdvancePipeline);
        ResetButton.onClick.AddListener(ResetExecution);
    }

    void AdvancePipeline()
    {
        if (isHalted) // Si el programa ha terminado, no avanzar
        {
            Debug.Log("El programa ha terminado. Presiona Reset para reiniciar.");
            return;
        }

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

        // Si hay más instrucciones, agregar una nueva al pipeline
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

        cycle++;
        UpdateUI();
        UpdateArrows();
    }

    void ExecuteInstruction(string instruction, int index)
    {
        switch (instruction)
        {
            case "LOAD R1":
                accumulator = inputPairs[0][0]; // Cargar el primer número del primer par
                break;
            case "LOAD R2":
                accumulator = inputPairs[0][1]; // Cargar el segundo número del primer par
                break;
            case "ADD R1, R2":
                accumulator = inputPairs[0][0] + inputPairs[0][1]; // Sumar los números del primer par
                break;
            case "LOAD R3":
                accumulator = inputPairs[1][0]; // Cargar el primer número del segundo par
                break;
            case "LOAD R4":
                accumulator = inputPairs[1][1]; // Cargar el segundo número del segundo par
                break;
            case "ADD R3, R4":
                accumulator = inputPairs[1][0] + inputPairs[1][1]; // Sumar los números del segundo par
                break;
            case "STORE ACC":
                if (accumulator == inputPairs[0][0] + inputPairs[0][1]) // Primera suma (primer par)
                {
                    memory[3] = "RESULT 1 = " + accumulator; // Guardar el resultado del primer par
                }
                else if (accumulator == inputPairs[1][0] + inputPairs[1][1]) // Segunda suma (segundo par)
                {
                    memory[7] = "RESULT 2 = " + accumulator; // Guardar el resultado del segundo par
                }
                break;
            case "HALT":
                isHalted = true; // Marcar que el programa ha terminado
                Debug.Log("Ejecución finalizada.");
                break;
        }
    }

    void ReadNumbers()
    {
        if (int.TryParse(InputNumber1.text, out int n1) && int.TryParse(InputNumber2.text, out int n2))
        {
            inputPairs = new int[2][]
            {
                new int[] { n1, n2 },         // Primer par
                new int[] { n1 + 1, n2 + 1 }  // Segundo par
            };

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
            UpdateUI();
        }
        else
        {
            Debug.Log("Por favor, ingresa números válidos.");
        }
    }

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
        InputNumber1.text = "";
        InputNumber2.text = "";
        UpdateUI();
    }

    void UpdateUI()
    {
        CPUText.text = $"CPU\nCiclo: {cycle}\nAcumulador: {accumulator}";
        MemoryText.text = "Memoria:\n" + string.Join("\n", memory);
        AccumulatorText.text = $"Acumulador\nValor: {accumulator}";
    }

    void UpdateArrows()
    {
        FetchArrow.enabled = false;
        DecodeArrow.enabled = false;
        LoadArrow.enabled = false;
        AddArrow.enabled = false;
        HaltArrow.enabled = false;

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
                            LoadArrow.enabled = true; // Activar la flecha de STORE
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
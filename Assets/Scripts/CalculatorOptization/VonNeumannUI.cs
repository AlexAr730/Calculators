using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VonNeumannUI : MonoBehaviour
{
    public TextMeshProUGUI CPUText;
    public TextMeshProUGUI MemoryText;
    public TextMeshProUGUI AccumulatorText;
    public TMP_InputField InputNumber1;
    public TMP_InputField InputNumber2;
    public Button NextStepButton;
    public Button ResetButton;

    private int pc = 0;
    private int accumulator = 0;
    private string operation = "None";
    private int number1 = 0;
    private int number2 = 0;
    private string[] memory;
    private bool numbersLoaded = false;

    void Start()
    {
        ResetExecution();
        NextStepButton.onClick.AddListener(AdvanceStep);
        ResetButton.onClick.AddListener(ResetExecution);
    }

    void AdvanceStep()
    {
        if (!numbersLoaded)
        {
            ReadNumbers();
            return;
        }

        if (pc < memory.Length)
        {
            string instruction = memory[pc];
            switch (instruction)
            {
                case "LOAD R1":
                    accumulator = number1;
                    operation = "LOAD";
                    break;
                case "LOAD R2":
                    accumulator = number2;
                    operation = "LOAD";
                    break;
                case "ADD R1, R2":
                    accumulator = number1 + number2;
                    operation = "ADD";
                    break;
                case "STORE ACC":
                    memory[3] = "RESULT = " + accumulator;
                    operation = "STORE";
                    break;
                case "HALT":
                    operation = "STOP";
                    break;
            }
            pc++;
        }

        UpdateUI();
    }

    void ReadNumbers()
    {
        if (int.TryParse(InputNumber1.text, out number1) && int.TryParse(InputNumber2.text, out number2))
        {
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
            Debug.Log("Por favor, ingresa números válidos.");
        }
    }

    void ResetExecution()
    {
        pc = 0;
        accumulator = 0;
        operation = "None";
        memory = new string[5] { "?", "?", "?", "?", "?" };
        InputNumber1.text = "";
        InputNumber2.text = "";
        numbersLoaded = false;
        UpdateUI();
    }

    void UpdateUI()
    {
        CPUText.text = $"CPU\nContador: {pc}\nOperación: {operation}";
        MemoryText.text = "Memoria:\n" + string.Join("\n", memory);
        AccumulatorText.text = $"Acumulador\nValor: {accumulator}";
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardID : MonoBehaviour
{
    [SerializeField] private RectTransform keyboardTransform;
    [SerializeField] private TMP_InputField inputFieldObjective;
    [SerializeField] private Button[] buttons;
    [SerializeField] private RectTransform canvasRectTransform;

    [SerializeField] private Selectable[] hideObjectKeyboard;

    private Vector2 offset;
    private bool keyboardActive = false;
    private Vector3 initialPosition;

    void Start()
    {
        if (keyboardTransform == null) keyboardTransform = GetComponent<RectTransform>();
        if (canvasRectTransform == null)
            canvasRectTransform = keyboardTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        initialPosition = keyboardTransform.anchoredPosition;
        keyboardTransform.gameObject.SetActive(false);

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; 

            if (i == buttons.Length - 1)
            {
                buttons[i].onClick.AddListener(DeleteCharacterKeyboard);
            }
            else
            {
                int numero = (i + 1) % 10;
                buttons[i].onClick.AddListener(() => AddNumbreKeyBoard(numero.ToString()));
            }
        }

        AddTrigger(inputFieldObjective.gameObject, EventTriggerType.Select, (_) => ShowKeyboard());

        foreach (Selectable obj in hideObjectKeyboard)
        {
            if (obj != null)
                AddTrigger(obj.gameObject, EventTriggerType.Select, (_) => HideKeyboard());
        }
    }

    void ShowKeyboard()
    {
        keyboardTransform.anchoredPosition = initialPosition;
        keyboardTransform.gameObject.SetActive(true);
        keyboardActive = true;
    }

    void HideKeyboard()
    {
        keyboardTransform.gameObject.SetActive(false);
        keyboardActive = false;
    }

    void AddNumbreKeyBoard(string numero)
    {
        if (inputFieldObjective != null)
            inputFieldObjective.text += numero;
    }

    void DeleteCharacterKeyboard()
    {
        if (inputFieldObjective != null && inputFieldObjective.text.Length > 0)
        {
            inputFieldObjective.text = inputFieldObjective.text.Substring(0, inputFieldObjective.text.Length - 1);
        }
    }

    private void AddTrigger(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> accion)
    {
        EventTrigger tr = obj.GetComponent<EventTrigger>();
        if (tr == null) tr = obj.AddComponent<EventTrigger>();

        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(accion);
        tr.triggers.Add(entry);
    }
}

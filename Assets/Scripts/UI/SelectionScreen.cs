using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SelectionScreen : MonoBehaviour
{
    [SerializeField] private List<ModificationSO> startingModifications;

    [SerializeField] private List<ModificationNode> modificationNode;

    private List<ModificationSO> tempModifications;
    private CanvasGroup cv;

    private bool canInteract = false;

    public List<IModification> currentlyActiveModifications;

    private void Awake()
    {
        cv = GetComponent<CanvasGroup>();

        currentlyActiveModifications = new List<IModification>();
        tempModifications = new List<ModificationSO>(startingModifications);
    }

    public void Open()
    {
        if (startingModifications.Count == 0) return;

        List<ModificationSO> currentModifications = new List<ModificationSO>(startingModifications);

        foreach (var modificationNode in modificationNode)
        {
            if (currentModifications.Count > 0)
            {
                modificationNode.gameObject.SetActive(true);

                ModificationSO modificationSO = currentModifications[Random.Range(0, currentModifications.Count)];

                modificationSO.Modification.Init();

                modificationNode.titleText.text = modificationSO.Modification.Title();
                modificationNode.descriptionText.text = modificationSO.Modification.Description();

                modificationNode.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                modificationNode.gameObject.GetComponent<Button>().onClick.AddListener(() => {

                    modificationSO.Modification.ApplyModification();

                    startingModifications.Remove(modificationSO);

                    startingModifications.AddRange(modificationSO.Modification.UnlockedModifications());

                    Close();

                    modificationNode.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                });

                currentModifications.Remove(modificationSO);
            }
            else
            {
                modificationNode.gameObject.SetActive(false);
            }
        }

        cv.DOFade(1, 0.3f).SetUpdate(true);
        cv.interactable = true;
        cv.blocksRaycasts = true;

        GameController.instance.Pause(true);

        DOVirtual.DelayedCall(0f, () => canInteract = true).SetUpdate(true);
    }

    public void Close()
    {
        if (!canInteract) return;

        cv.DOFade(0, 0.3f).SetUpdate(true);
        cv.interactable = false;
        cv.blocksRaycasts = false;

        canInteract = false;

        GameController.instance.Pause(false);
    }

    public void Restart()
    {
        startingModifications = new List<ModificationSO>(tempModifications);

        currentlyActiveModifications.Clear();
    }
}
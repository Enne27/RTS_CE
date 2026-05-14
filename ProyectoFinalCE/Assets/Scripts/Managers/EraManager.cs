using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using static PlayerConstants;

public class EraManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] Sprite broteSprite;
    [SerializeField] Sprite nidoSprite;
    [SerializeField] Sprite coloniaSprite;
    [SerializeField] Sprite imperioSprite;

    [HideInInspector] public Dictionary<HIVE_ERAS, Sprite> ERAS_IMAGES;
    private Dictionary<HIVE_ERAS, LocalizedString> ERAS_LOCALIZED;

    [Header("UI")]
    private GeneralInfoView generalInfoView;

    [Header("Localization")]
    [SerializeField] LocalizedString brote_ls;
    [SerializeField] LocalizedString nido_ls;
    [SerializeField] LocalizedString colonia_ls;
    [SerializeField] LocalizedString imperio_ls;
    #endregion

    #region SINGLETON
    public static EraManager instance { get; private set; }

    #endregion


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        ERAS_IMAGES = new()
        {
            { HIVE_ERAS.BROTE,  broteSprite},
            { HIVE_ERAS.NIDO,  nidoSprite},
            { HIVE_ERAS.COLONIA, coloniaSprite },
            { HIVE_ERAS.IMPERIO,  imperioSprite}
        };

        ERAS_LOCALIZED = new()
        {
            { HIVE_ERAS.BROTE, brote_ls },
            { HIVE_ERAS.NIDO, nido_ls },
            { HIVE_ERAS.COLONIA, colonia_ls },
            { HIVE_ERAS.IMPERIO, imperio_ls }
        };

        generalInfoView = FindFirstObjectByType<GeneralInfoView>();
    }

    public void AdvanceEra(bool isPlayer)
    {
        if (isPlayer)
        {
            GameManager.instance.player.currentEra += 1;
            ChangesNewEra();
        }
        else GameManager.instance.playerIA.currentEra += 1;
    }

    public void ChangesNewEra()
    {
        HIVE_ERAS era = GameManager.instance.player.currentEra;

        LocalizedString newEraName = ERAS_LOCALIZED[era];

        generalInfoView.UpdateCurrentEraVisuals(era, newEraName);
    }

}

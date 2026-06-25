using Unity.Netcode;
using UnityEngine;

public class TankSkinLoader : NetworkBehaviour
{
    [Header("Tank Renderers")]
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer turretRenderer;
    [SerializeField] private Animator baseAnimator;

    [SerializeField] private TankSkinLibrary skinLibrary;

    private NetworkVariable<int> networkSkinIndex = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Color> networkTankColor = new NetworkVariable<Color>(
        Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool _skinDirty = false;
    private bool _colorDirty = false;

    private void Awake()
    {
        if (baseRenderer) baseRenderer.forceRenderingOff = false;
        if (turretRenderer) turretRenderer.forceRenderingOff = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        networkSkinIndex.OnValueChanged += OnSkinChanged;
        networkTankColor.OnValueChanged += OnColorChanged;

        if (IsOwner)
        {
            int skinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);

            float r = PlayerPrefs.GetFloat("ColorR", 1f);
            float g = PlayerPrefs.GetFloat("ColorG", 1f);
            float b = PlayerPrefs.GetFloat("ColorB", 1f);
            Color savedColor = new Color(r, g, b);

            SetTankSkinServerRpc(skinIndex, savedColor);
            ApplySkin(skinIndex, savedColor);
        }
        else
        {
            ApplySkin(networkSkinIndex.Value, networkTankColor.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        networkSkinIndex.OnValueChanged -= OnSkinChanged;
        networkTankColor.OnValueChanged -= OnColorChanged;
        base.OnNetworkDespawn();
    }

    private void OnSkinChanged(int oldSkin, int newSkin)
    {
        _skinDirty = true;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        _colorDirty = true;
    }

    private void LateUpdate()
    {
        if (_skinDirty || _colorDirty)
        {
            ApplySkin(networkSkinIndex.Value, networkTankColor.Value);
            _skinDirty = false;
            _colorDirty = false;
        }
    }

    private void ApplySkin(int skinIndex, Color color)
    {
        if (skinLibrary == null || skinLibrary.allSkins.Count == 0) return;
        
        TankSkin skin = skinLibrary.allSkins[skinIndex];

        if (baseRenderer)
        {
            baseRenderer.sprite = skin.baseSprite;
            baseRenderer.color = color;
        }
        if (turretRenderer)
        {
            turretRenderer.sprite = skin.turretSprite;
            turretRenderer.color = color;
        }

        if(baseAnimator)
        {
            baseAnimator.runtimeAnimatorController = skin.baseAnimator;
        }
    }

    [Rpc(SendTo.Server)]
    private void SetTankSkinServerRpc(int skinIndex, Color color)
    {
        networkSkinIndex.Value = skinIndex;
        networkTankColor.Value = color;
    }
}
using Unity.Netcode;
using UnityEngine;

public class TankSkinLoader : NetworkBehaviour
{
    [Header("Tank Renderers")]
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private SpriteRenderer turretRenderer;
    [SerializeField] private Animator baseAnimator;

    [SerializeField] private TankSkinLibrary skinLibrary;
    [SerializeField] private TurretSkinLibrary turretSkinLibrary;

    private NetworkVariable<int> networkSkinIndex = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Color> networkTankColor = new NetworkVariable<Color>(
        Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<WeaponType> networkWeaponType = new NetworkVariable<WeaponType>(
        WeaponType.Normal, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool _skinDirty = false;
    private bool _colorDirty = false;
    private bool _weaponDirty = false;

    private void Awake()
    {
        if (baseRenderer) baseRenderer.forceRenderingOff = false;
        if (turretRenderer) turretRenderer.forceRenderingOff = false;
    }

    public override void OnNetworkSpawn()
    {
        networkSkinIndex.OnValueChanged += OnSkinChanged;
        networkTankColor.OnValueChanged += OnColorChanged;
        networkWeaponType.OnValueChanged += OnWeaponChanged;

        if (IsOwner)
        {
            int skinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);

            float r = PlayerPrefs.GetFloat("ColorR", 1f);
            float g = PlayerPrefs.GetFloat("ColorG", 1f);
            float b = PlayerPrefs.GetFloat("ColorB", 1f);
            Color savedColor = new Color(r, g, b);

            SetTankSkinServerRpc(skinIndex, savedColor);
            ApplySkin(skinIndex, savedColor, WeaponType.Normal);
        }
        else
        {
            ApplySkin(networkSkinIndex.Value, networkTankColor.Value, networkWeaponType.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        networkSkinIndex.OnValueChanged -= OnSkinChanged;
        networkTankColor.OnValueChanged -= OnColorChanged;
        networkWeaponType.OnValueChanged -= OnWeaponChanged;
    }

    private void OnSkinChanged(int oldSkin, int newSkin)
    {
        _skinDirty = true;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        _colorDirty = true;
    }

    private void OnWeaponChanged(WeaponType oldWeapon, WeaponType newWeapon)
    {
        _weaponDirty = true;
    }

    private void LateUpdate()
    {
        if (_skinDirty || _colorDirty || _weaponDirty)
        {
            ApplySkin(networkSkinIndex.Value, networkTankColor.Value, networkWeaponType.Value);
            _skinDirty = false;
            _colorDirty = false;
            _weaponDirty = false;
        }
    }

    private void ApplySkin(int skinIndex, Color color, WeaponType weapon)
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
            if(weapon == WeaponType.Gatling || weapon == WeaponType.Laser)
            {
                turretRenderer.sprite = turretSkinLibrary.allTurrets.Find(t => t.turretName == weapon.ToString()).turretSprite;
            }
            else
            {
                turretRenderer.sprite = skin.turretSprite;
            }
            turretRenderer.color = color;
        }

        if(baseAnimator)
        {
            if (skin.baseAnimator)
            {
                baseAnimator.enabled = true;
                baseAnimator.runtimeAnimatorController = skin.baseAnimator;
            }
            else
            {
                baseAnimator.runtimeAnimatorController = null;
                baseAnimator.enabled = false;
                baseRenderer.sprite = skin.baseSprite;
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SetTankSkinServerRpc(int skinIndex, Color color)
    {
        networkSkinIndex.Value = skinIndex;
        networkTankColor.Value = color;
    }

    [Rpc(SendTo.Server)]
    public void SetWeaponTypeServerRpc(WeaponType weaponType)
    {
        networkWeaponType.Value = weaponType;
    }
}
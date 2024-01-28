using Fusion;
using UnityEngine;

public class NetworkObjectToHide : NetworkBehaviour
{
    private GameObject objectToHide;

    public override void Spawned()
    {
        objectToHide = gameObject;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void HideObjectRpc()
    {
        objectToHide.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void ShowObjectRpc()
    {
        objectToHide.SetActive(true);
    }
}
using System.Collections;
using Fusion;
using UnityEngine;

public class ItemPickable : NetworkBehaviour
{
    [SerializeField] public float pickupImpulse = 2f;
    [SerializeField] public float deleteDelay = 1f;
    [SerializeField] public GameController gameController;
    [SerializeField] public GameObject particles;

    private Rigidbody2D _rigidbody2D;
    
    private ChangeDetector _changeDetector;

    [Networked] private bool isDeletingNow { get; set; }

    public override void Spawned()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public void PickUp()
    {
        if (!isDeletingNow)
        {
            particles.SetActive(true);
            isDeletingNow = true;
            _rigidbody2D.AddForce(Vector2.up * pickupImpulse, ForceMode2D.Impulse);
            gameController.IncrementPapersCountRpc();
            StartCoroutine(DeleteWithDelay());
        }
    }

    private IEnumerator DeleteWithDelay()
    {
        yield return new WaitForSeconds(deleteDelay);
        Destroy(gameObject);
    }

    void Update()
    {
        if (HasStateAuthority || _changeDetector == null)
        {
            return;
        }

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(isDeletingNow):
                    PickUp();
                    break;
            }
        }
    }
}
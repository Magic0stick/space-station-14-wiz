using Content.Shared.Verbs;
using Robust.Shared.Player;
using Content.Server.Administration;
using Content.Server.Prayer;
using Content.Shared.Revenant.Components;

namespace Content.Server.Revenant;

public sealed partial class TelepathySystem : EntitySystem
{
    [Dependency] private readonly QuickDialogSystem _quickDialog = default!;
    [Dependency] private readonly PrayerSystem _prayerSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<GetVerbsEvent<Verb>>(GetVerbs);
    }

    private void GetVerbs(GetVerbsEvent<Verb> ev)
    {
        AddRevenantVerbs(ev);
    }

    private void AddRevenantVerbs(GetVerbsEvent<Verb> args)
    {
        if (!EntityManager.TryGetComponent(args.User, out ActorComponent? actor))
            return;

        var player = actor.PlayerSession;

        if (HasComp<RevenantComponent>(args.User))
        {

            if (TryComp(args.Target, out ActorComponent? targetActor))
            {
                // Subtle Messages
                Verb telepathy = new();
                telepathy.Text = Loc.GetString("Пробратся в мысли");
                telepathy.Priority = -3;
                telepathy.DoContactInteraction = true;
                telepathy.Act = () =>
                {
                    _quickDialog.OpenDialog(player, "Пробратся в мысли", "Сообщение", (string message) =>
                    {
                        _prayerSystem.SendSubtleMessage(targetActor.PlayerSession, player, message, Loc.GetString("prayer-popup-subtle-default"));
                    });
                };
                args.Verbs.Add(telepathy);
            }
        }
    }
}



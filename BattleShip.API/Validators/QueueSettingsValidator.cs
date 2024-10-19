using BattleShip.Models;
using FluentValidation;

public class QueueSettingsValidator : AbstractValidator<QueueSettings>
{
    public QueueSettingsValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Queue type is required.")
            .Must(BeAValidQueueType).WithMessage("Invalid queue type.");

        RuleFor(x => x.AIDifficulty)
            .Cascade(CascadeMode.Stop)
            .Must((settings, aidifficulty) => settings.Type != QueueType.AgainstAI.ToString() || (aidifficulty >= 0 && aidifficulty <= 1))
            .WithMessage("AIDifficulty must be between 0 and 2 if QueueType is AgainstAI.")
            .Must((settings, aidifficulty) => settings.Type == QueueType.AgainstAI.ToString() || aidifficulty == null)
            .WithMessage("AIDifficulty must be null if QueueType is not AgainstAI.");
    }

    private bool BeAValidQueueType(string type)
    {
        return type == QueueType.QuickPlay.ToString() || type == QueueType.AgainstAI.ToString();
    }
}
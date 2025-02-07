
/*public class BuffFactory
{
    public static BuffBase CreateBuff(string name, UnitBase c, BuffManager manager)
    {
        BuffBase buff = null;
        if (name == "poisoned")
            buff = CreatePoisoned(c, manager);
        if (name == "hemophagia")
            buff = CreateHemophagia(c, manager);
        return buff;
    }
    private static BuffBase CreatePoisoned(CombatEntity c, BuffManager manager)
    {
        NewBuffBase poisoned = new PoisonedBuff(manager, c);
        return poisoned;
    }
    private static BuffBase CreateHemophagia(CombatEntity c, BuffManager manager)
    {
        NewBuffBase hemophagia = new HemophagiaBuff(manager, c);
        return hemophagia;
    }
}*/
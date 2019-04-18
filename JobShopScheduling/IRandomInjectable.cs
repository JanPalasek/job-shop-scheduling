namespace JobShopScheduling
{
    using System;

    public interface IRandomInjectable
    {
        void InjectRandom(Random random);
    }
}
using System.Threading.Tasks;
using AElf.Contracts.Association;
using AElf.CSharp.Core.Extension;
using AElf.Kernel;
using AElf.Types;
using BlockChainKit.AElf;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Governing.AElf
{
    class MemberAddedLogEventListener : LogEventListenerBase<MemberAdded>, ITransientDependency
    {
        public MemberAddedLogEventListener(IOptionsSnapshot<AElfEventListeningOptions> optionsSnapshot,
            IAElfChainKit aelfChainKit) : base(optionsSnapshot.Value, aelfChainKit)
        {
            var memberAdded = new MemberAdded
            {
                OrganizationAddress = Address.FromBase58(AElfEventListeningOptions.InterestedOrganizationAddress)
            }.ToLogEvent(Address.FromBase58(AElfEventListeningOptions.ListeningContractAddress));
            ListeningEvent = new InterestedEvent
            {
                LogEvent = memberAdded,
                Bloom = memberAdded.GetBloom()
            };
        }

        protected override async Task<bool> IsInterestedEvent(LogEvent log)
        {
            var memberAdded = MemberAdded.Parser.ParseFrom(log.NonIndexed);
            return memberAdded.Member == await AElfChainKit.GetAccountAsync();
        }
    }
}
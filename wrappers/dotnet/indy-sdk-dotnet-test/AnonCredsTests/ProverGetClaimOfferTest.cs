﻿using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.Test.WalletTests;
using Hyperledger.Indy.WalletApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace Hyperledger.Indy.Test.AnonCredsTests
{
    [TestClass]
    public class ProverGetClaimOfferTest : AnonCredsIntegrationTestBase
    {
        [TestMethod]
        public async Task TestsProverGetClaimOffersWorksForEmptyFilter()
        {
            await InitCommonWallet();

            var claimOffers = await AnonCreds.ProverGetClaimOffersAsync(_commonWallet, "{}");
            var claimOffersArray = JArray.Parse(claimOffers);

            Assert.AreEqual(3, claimOffersArray.Count);
        }

        [TestMethod]
        public async Task TestsProverGetClaimOffersWorksForFilterByIssuer()
        {
            await InitCommonWallet();

            var filter = string.Format("{{\"issuer_did\":\"{0}\"}}", _issuerDid);

            var claimOffers = await AnonCreds.ProverGetClaimOffersAsync(_commonWallet, filter);
            var claimOffersArray = JArray.Parse(claimOffers);

            Assert.AreEqual(2, claimOffersArray.Count);

            Assert.IsTrue(claimOffers.Contains(string.Format(_claimOfferTemplate, _issuerDid, 1)));
            Assert.IsTrue(claimOffers.Contains(string.Format(_claimOfferTemplate, _issuerDid, 2)));
        }

        [TestMethod] 
        public async Task TestsProverGetClaimOffersWorksForFilterBySchema()
        {
            await InitCommonWallet();

            var filter = string.Format("{{\"schema_seq_no\":{0}}}", 2);

            var claimOffers = await AnonCreds.ProverGetClaimOffersAsync(_commonWallet, filter);
            var claimOffersArray = JArray.Parse(claimOffers);

            Assert.AreEqual(2, claimOffersArray.Count);

            Assert.IsTrue(claimOffers.Contains(string.Format(_claimOfferTemplate, _issuerDid, 2)));
            Assert.IsTrue(claimOffers.Contains(string.Format(_claimOfferTemplate, _issuerDid2, 2)));
        }

        [TestMethod] 
        public async Task TestsProverGetClaimOffersWorksForFilterByIssuerAndSchema()
        {
            await InitCommonWallet();

            var filter = string.Format("{{\"issuer_did\":\"{0}\",\"schema_seq_no\":{1}}}", _issuerDid, 1);

            var claimOffers = await AnonCreds.ProverGetClaimOffersAsync(_commonWallet, filter);
            var claimOffersArray = JArray.Parse(claimOffers);

            Assert.AreEqual(1, claimOffersArray.Count);

            Assert.IsTrue(claimOffers.Contains(string.Format(_claimOfferTemplate, _issuerDid, 1)));
        }

        [TestMethod]
        public async Task TestsProverGetClaimOffersWorksForNoResult()
        {
            await InitCommonWallet();

            var filter = string.Format("{{\"schema_seq_no\":{0}}}", 3);

            var claimOffers = await AnonCreds.ProverGetClaimOffersAsync(_commonWallet, filter);
            var claimOffersArray = JArray.Parse(claimOffers);

            Assert.AreEqual(0, claimOffersArray.Count);
        }

        [TestMethod]
        public async Task TestProverGetClaimOffersWorksForInvalidFilterJson()
        {
            await InitCommonWallet();

            var filter = string.Format("{{\"schema_seq_no\":\"{0}\"}}", 1);

            var ex = await Assert.ThrowsExceptionAsync<IndyException>(() =>
                AnonCreds.ProverGetClaimOffersAsync(_commonWallet, filter)

            );

            Assert.AreEqual(ErrorCode.CommonInvalidStructure, ex.ErrorCode);
        }

        [TestMethod]
        public async Task TestGetClaimOffersForPlugged()
        {
            var type = "proverInmem";
            var poolName = "default";
            var walletName = "proverCustomWallet";

            await Wallet.RegisterWalletTypeAsync(type, new InMemWalletType());
            await Wallet.CreateWalletAsync(poolName, walletName, type, null, null);

            string claimOffers;
            Wallet wallet = null;

            var claimOffer = string.Format(_claimOfferTemplate, _issuerDid, 1);
            var claimOffer2 = string.Format(_claimOfferTemplate, _issuerDid, 2);
            var claimOffer3 = string.Format(_claimOfferTemplate, _issuerDid2, 2);

            try
            {
                wallet = await Wallet.OpenWalletAsync(walletName, null, null);

                await AnonCreds.ProverStoreClaimOfferAsync(wallet, claimOffer);
                await AnonCreds.ProverStoreClaimOfferAsync(wallet, claimOffer2);
                await AnonCreds.ProverStoreClaimOfferAsync(wallet, claimOffer3);

                var filter = string.Format("{{\"issuer_did\":\"{0}\"}}", _issuerDid);

                claimOffers = await AnonCreds.ProverGetClaimOffersAsync(wallet, filter);
            }
            finally
            {
                if (wallet != null)
                    await wallet.CloseAsync();

                await Wallet.DeleteWalletAsync(walletName, null);
            }

            var claimOffersArray = JArray.Parse(claimOffers);

            Assert.AreEqual(2, claimOffersArray.Count);
            Assert.IsTrue(claimOffers.Contains(claimOffer));
            Assert.IsTrue(claimOffers.Contains(claimOffer2));

        }

    }
}

package org.hyperledger.indy.sdk.anoncreds;

import org.hyperledger.indy.sdk.ErrorCode;
import org.hyperledger.indy.sdk.ErrorCodeMatcher;
import org.hyperledger.indy.sdk.wallet.Wallet;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;

import java.util.concurrent.ExecutionException;

public class ProverStoreClaimOfferTest extends AnoncredsIntegrationTest {

	private Wallet wallet;
	private String walletName = "storeClaimOfferWallet";

	@Before
	public void createWallet() throws Exception {
		Wallet.createWallet("default", walletName, "default", null, null).get();
		this.wallet = Wallet.openWallet(walletName, null, null).get();
	}

	@After
	public void deleteWallet() throws Exception {
		wallet.closeWallet().get();
		Wallet.deleteWallet(walletName, null).get();
	}

	@Test
	public void testProverStoreClaimOfferWorks() throws Exception {

		String claimOffer = "{\"issuer_did\":\"NcYxiDXkpYi6ov5FcYDi1e\",\"schema_seq_no\":1 }";

		Anoncreds.proverStoreClaimOffer(wallet, claimOffer).get();
	}

	@Test
	public void testProverStoreClaimOfferWorksForInvalidJson() throws Exception {

		thrown.expect(ExecutionException.class);
		thrown.expectCause(new ErrorCodeMatcher(ErrorCode.CommonInvalidStructure));

		String claimOffer = "{\"issuer_did\":\"NcYxiDXkpYi6ov5FcYDi1e\"}";

		Anoncreds.proverStoreClaimOffer(wallet, claimOffer).get();
	}

	@Test
	public void testProverStoreClaimOfferWorksForInvalidIssuerDid() throws Exception {

		thrown.expect(ExecutionException.class);
		thrown.expectCause(new ErrorCodeMatcher(ErrorCode.CommonInvalidStructure));

		String claimOffer = "{\"issuer_did\":\"invalid_base58_string\",\"schema_seq_no\":1}";

		Anoncreds.proverStoreClaimOffer(wallet, claimOffer).get();
	}
}

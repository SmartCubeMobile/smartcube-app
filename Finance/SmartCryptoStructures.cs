using System;
using System.Text.Json.Serialization;

namespace SmartCubeMobile
{
#if CRYPTOS
    public class ETHLedgerTransaction
    {
        public string BlockHash { get; set; }
        public int BlockNumber { get; set; }
        public string Confirmations { get; set; }
        public string ContactAddress { get; set; }
        public decimal CumulativeGasUsed { get; set; }
        public string From { get; set; }
        public string FunctionName { get; set; }
        public decimal Gas { get; set; }
        public decimal GasPrice { get; set; }
        public decimal GasUsed { get; set; }
        public string Hash { get; set; }
        public string Input { get; set; }
        public short IsError { get; set; }
        public string MethodId { get; set; }
        public int Nonce { get; set; }
        public DateTime TransactionDate { get; set; }
        public string To { get; set; }
        public int TransactionIndex { get; set; }
        public int TxReceiptStatus { get; set; }
        public decimal Value { get; set; }
        public string NetworkName { get; set; }
    }
    public class XRPLedgerTransaction
    {
        public string Account { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public long Date { get; set; }
        public string DeliverMax { get; set; }
        public string Destination { get; set; }
        public string Fee { get; set; }
        public string Flags { get; set; }
        public string Hash { get; set; }
        public string InLedger { get; set; }
        public string LasLedgerSequence { get; set; }
        public string LedgerIndex { get; set; }
        public string Memos { get; set; }
        public string Sequence { get; set; }
        public string SigningPubKey { get; set; }
        public string TransactionType { get; set; }
        public string TransactionSignature { get; set; }
        public string NetworkName { get; set; }
        public bool Validated { get; set; }
    }


    // Class representing the JSON-RPC request structure
    public class XRPRequestTX
    {
        [JsonPropertyName("command")]
        public string command { get; set; }

        [JsonPropertyName("transaction")]
        public string transaction { get; set; }

        [JsonPropertyName("binary")]
        public bool binary { get; set; }

        [JsonPropertyName("api_version")]
        public int api_version { get; set; }
    }
    public class XRPRequestWebSocket
    {
        [JsonPropertyName("command")]
        public string command { get; set; }

        [JsonPropertyName("account")]
        public string account { get; set; }

        [JsonPropertyName("ledger_index_min")]
        public int ledger_index_min { get; set; }
        [JsonPropertyName("ledger_index_max")]
        public int ledger_index_max { get; set; }

        [JsonPropertyName("binary")]
        public bool binary { get; set; }
        [JsonPropertyName("limit")]
        public int limit { get; set; }
        [JsonPropertyName("forward")]
        public bool forward { get; set; }

        [JsonPropertyName("api_version")]
        public int api_version { get; set; }
    }

    public class XRPRequest
    {
        [JsonPropertyName("account")]
        public string account { get; set; }

        [JsonPropertyName("strict")]
        public bool strict { get; set; }
        [JsonPropertyName("ledgr_index")]
        public string ledger_index { get; set; }

        [JsonPropertyName("queue")]
        public bool queue { get; set; }

    }

    public class SmartCrypto
    {
        public class Wallet
        {
            public string USERNAME
            { get; set; }
            public char CUBEFACE_CODE
            { get; set; }
            // There is no 'SOURCE' as it repeatedly changes
            // and is usually the Account no
            public string CRYPTO_DESTINATION
            { get; set; }
            public string CRYPTO_WALLET_NAME
            { get; set; }
            public short CRYPTO_CURRENCY_ORDINAL
            { get; set; }
            public string CRYPTO_CURRENCY
            { get; set; }
            public double CRYPTO_AMOUNT
            { get; set; }

        }

        public class CurrencyTotals
        {
            public int INDEX
            { get; set; }
            public short CRYPTO_CURRENCY_ORDINAL
            { get; set; }
            public string CRYPTO_CURRENCY
            { get; set; }
            public double CRYPTO_AMOUNT
            { get; set; }
            public double NATIVE_AMOUNT
            { get; set; }
            public string NATIVE_CURRENCY
            { get; set; }
            public double FEE_AMOUNT
            { get; set; }
            public string FEE_CURRENCY
            { get; set; }
            public double RATE
            { get; set; }
            public string VALUE
            { get; set; }
        }

        public class LedgerTransactionView
        {
            public string LEDGER_NAME
            { get; set; }
            public string WALLET
            { get; set; }
            public string ACCOUNT
            { get; set; }
            public DateTime TRANSACTION_DATE
            { get; set; }
            public double AMOUNT
            { get; set; }
            public string SOURCE
            { get; set; }
            public string DESTINATION
            { get; set; }
            public short CRYPTO_CURRENCY_ORDINAL
            { get; set; }
            public string CURRENCY_DISPLAY
            { get; set; }
            //public string DeliverMax { get; set; }

            public string FEE
            { get; set; }
            //public string Flags { get; set; }
            //public string Hash { get; set; }
            //public string InLedger { get; set; }
            //public string LasLedgerSequence { get; set; }
            //public string LedgerIndex { get; set; }
            //public string Memos { get; set; }
            //public string Sequence { get; set; }
            //public string SigningPubKey { get; set; }
            public string TRANSACTION_TYPE
            { get; set; }
            public string TO_ADDRESS
            { get; set; }
            public string NETWORK_NAME
            { get; set; }
            public string HASH
            { get; set; }
            //public string TransactionSignature { get; set; }

            //public bool Validated { get; set; }

        }
        public class V2CryptoTotals
        {
            public string
                CRYPTO_CURRENCY
            { get; set; }
            public short
                CRYPTO_CURRENCY_ORDINAL
            { get; set; }
            public double
                CRYPTO_AMOUNT
            { get; set; }
            public double
                VALUE
            { get; set; }
            public double
                RATE
            { get; set; }
        }

        public struct V2User
        {
            public string
                USERNAME;
            public string
                COUNTRY;
            public string
                EMAIL;
            public string
                NAME;
            public DateTime
                CREATED_AT;
            public string
                NATIVE_CURRENCY;
            public string
                ID;
            public string
                BITCOIN_UNIT;
            public string
                RESOURCE;
            public string
                RESOURCE_PATH;
            public TimeZoneInfo
                TIME_ZONE;

        }

        public struct V2Currencies
        {
            public string
                ID;
            public string
                MIN_SIZE;
            public string
                NAME;
        }

        internal struct V2ExchangeRates
        {
            internal string
                NAMEX
            { get; set; }
            internal string
                RATE
            { get; set; }
            internal short
                NAME_ORDINAL
            { get; set; }
        }

        internal struct V2AccountRewards
        {
            internal string
                APY
            { get; set; }
            internal string
                FORMATTED_APY
            { get; set; }
            internal string
                LABEL
            { get; set; }
        }
        internal struct V2AccountCurrency
        {
            internal string
                ASSET_ID
            { get; set; }
            internal string
                CODE
            { get; set; }
            internal string
                COLOR
            { get; set; }
            internal string
                EXPONENT
            { get; set; }
            internal string
                NAME
            { get; set; }
            internal V2AccountRewards
                REWARDS
            { get; set; }
            internal string
                SLUG
            { get; set; }
            internal string
                TYPE
            { get; set; }
        }

        internal struct V2Accounts
        {
            internal string
                USERNAME
            { get; set; }
            internal string
                EXCHANGE
            { get; set; }
            internal string
                ALLOW_DEPOSITS
            { get; set; }
            internal string
                ALLOW_WITHDRAWALS
            { get; set; }
            internal V2Amount
                BALANCE
            { get; set; }
            internal DateTime
                CREATED_AT
            { get; set; }
            internal V2AccountCurrency
                CURRENCY
            { get; set; }
            internal string
                ID
            { get; set; }
            internal string
                NAME
            { get; set; }
            internal string
                PORTFOLIO_ID
            { get; set; }
            internal string
                PRIMARY
            { get; set; }
            internal string
                RESOURCE
            { get; set; }
            internal string
                RESOURCE_PATH
            { get; set; }
            internal string
                TYPE
            { get; set; }
            internal DateTime
                UPDATED_AT
            { get; set; }
            internal string
                UDPRN
            { get; set; }
        }

        internal struct V2ShareAddress
        {
            internal string
                LINE1
            { get; set; }
            internal string
                LINE2
            { get; set; }
        }

        internal struct V2InlineWarning
        {
            internal string
                TEXT
            { get; set; }
            internal string
                TOOLTIP
            { get; set; }
        }

        internal struct V2AddressInfo
        {
            internal string
                ADDRESS
            { get; set; }
        }
        internal struct V2Addresses
        {
            internal string
                USERNAME
            { get; set; }
            internal string
                EXCHANGE
            { get; set; }
            internal string
                UUID
            { get; set; }         // <= Account UUID => our ACCOUNT_NO
            internal string
                ADDRESS
            { get; set; }
            internal V2AddressInfo
                ADDRESS_INFO
            { get; set; }
            internal string
                ADDRESS_LABEL
            { get; set; }
            internal string
                CALLBACK_URL
            { get; set; }
            internal DateTime
                CREATED_AT
            { get; set; }
            internal string
                DEFAULT_RECEIVE
            { get; set; }
            internal string
                DEPOSIT_URI
            { get; set; }
            internal string
                DESTINATION_TAG
            { get; set; }
            internal string
                ID
            { get; set; }
            internal V2InlineWarning
                INLINE_WARNING
            { get; set; }
            internal string
                NAME
            { get; set; }
            internal string
                NETWORK
            { get; set; }
            internal string
                QR_CODE_IMAGE_URL
            { get; set; }
            internal string
                RECEIVE_SUBTITLE
            { get; set; }
            internal string
                RESOURCE
            { get; set; }
            internal string
                RESOURCE_PATH
            { get; set; }
            internal V2ShareAddress
                SHARE_ADDRESS_COPY
            { get; set; }
            internal DateTime
                UPDATED_AT
            { get; set; }
            internal string
                URI_SCHEME
            { get; set; }
            internal string             // Put in by Ray so we know which of Dan's its come from
                UDPRN
            { get; set; }
        }

        internal struct V2Amount
        {
            internal string
                AMOUNT 
            { get; set; } 
            internal string
                CURRENCY
            { get; set; }
        }
        internal struct V2BuySell
        {
            internal V2Amount FEE
            { get; set; }
            internal string ID
            { get; set; }
            internal string PAYMENT_METHOD_NAME
            { get; set; }
            internal V2Amount SUBTOTAL
            { get; set; }
            internal V2Amount TOTAL
            { get; set; }
        }

        internal struct V2TransactionNetwork
        {
            internal string HASH { get; set; }
            internal string NETWORK_NAME { get; set; }
            internal string STATUS { get; set; }
            internal string STATUS_DESCRIPTION { get; set; }
            internal string TRANSACTION_URL { get; set; }
            internal string TRANSACTION_FEE { get; set; }
        }

        internal struct V2To
        {
            internal string
                ADDRESS
            { get; set; }
            internal string
                RESOURCE
            { get; set; }
        }

        public class V2Transactions
        {
            internal string
                USERNAME
            { get; set; }
            internal string
                EXCHANGE
            { get; set; }
            internal string
                UUID
            { get; set; }          // <= Account UUID => our ACCOUNT_NO
            internal string
                UDPRN
            { get; set; }
            internal V2Amount
                AMOUNT
            {get; set; }
            internal V2BuySell
                BUYSELL
            { get; set; }
            public DateTime
                CREATED_AT
            { get; set; }
            internal string
                DESCRIPTION
            { get; set; }
            internal string
                ID
            { get; set; }
            internal string
                IDEM
            { get; set; }
            internal V2Amount
                NATIVE_AMOUNT
            { get; set; }
            internal V2TransactionNetwork
                NETWORK
            { get; set; }
            internal string
                RESOURCE
            { get; set; }
            internal string
                RESOURCE_PATH
            { get; set; }
            internal string
                STATUS
            { get; set; }
            internal V2To
                TO
            { get; set; }
            public string
                TYPE
            { get; set; }
            public DateTime
                ACCOUNT_CREATED
            { get; set; }
            internal bool Processed = false;
        }

        internal struct V3Value
        {
            internal string
                VALUE
            { get; set; }
            internal string
                CURRENCY
            { get; set; }
        }
        internal struct V3Accounts
        {
            internal string
                USERNAME
            { get; set; }
            internal string
                EXCHANGE { get; set; }
            internal string
                ACTIVE { get; set; }
            internal V3Value
                AVAILABLE_BALANCE
            { get; set; }
            internal DateTime
                CREATED_AT
            { get; set; }
            internal string
                CURRENCY
            { get; set; }
            internal string
                DEFAULT
            { get; set; }
            internal DateTime
                DELETED_AT
            { get; set; }
            internal V3Value
                HOLD
            { get; set; }
            internal string
                NAME
            { get; set; }
            internal string
                PLATFORM
            { get; set; }
            internal string
                READY
            { get; set; }
            internal string
                RETAIL_PORTFOLIO_ID
            { get; set; }
            internal string
                TYPE
            { get; set; }
            internal DateTime
                UPDATED_AT
            { get; set; }
            internal string
                UUID
            { get; set; }
            internal string     // Needed for Accounts in SmartSwitch!
                UDPRN
            { get; set; }
        }
    }
#endif
}
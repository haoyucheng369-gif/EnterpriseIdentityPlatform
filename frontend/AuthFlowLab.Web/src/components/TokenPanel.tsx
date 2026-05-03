type TokenPanelProps = {
  accessToken?: string;
  idToken?: string;
};

export function TokenPanel({ accessToken, idToken }: TokenPanelProps) {
  return (
    <section className="card content-card">
      <h2>Tokens</h2>
      <TokenBlock label="access_token" value={accessToken} />
      <TokenBlock label="id_token" value={idToken} />
    </section>
  );
}

function TokenBlock({ label, value }: { label: string; value?: string }) {
  return (
    <div className="token-block">
      <h3>{label}</h3>
      <pre>{value ?? '-'}</pre>
    </div>
  );
}

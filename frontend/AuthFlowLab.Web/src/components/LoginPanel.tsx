type LoginPanelProps = {
  username: string;
  password: string;
  message: string;
  onUsernameChange: (value: string) => void;
  onPasswordChange: (value: string) => void;
  onLogin: () => void;
  onClear: () => void;
};

export function LoginPanel({
  username,
  password,
  message,
  onUsernameChange,
  onPasswordChange,
  onLogin,
  onClear
}: LoginPanelProps) {
  return (
    <section className="card login-card">
      <div>
        <p className="eyebrow">OAuth2 / OIDC</p>
        <h1>AuthFlowLab</h1>
      </div>

      <label className="form-field">
        Username
        <input value={username} onChange={(event) => onUsernameChange(event.target.value)} />
      </label>

      <label className="form-field">
        Password
        <input value={password} onChange={(event) => onPasswordChange(event.target.value)} type="password" />
      </label>

      <div className="button-row">
        <button type="button" className="btn btn-primary" onClick={onLogin}>
          Login with PKCE
        </button>
        <button type="button" className="btn btn-outline" onClick={onClear}>
          Clear
        </button>
      </div>

      {message ? <p className="alert">{message}</p> : null}
    </section>
  );
}

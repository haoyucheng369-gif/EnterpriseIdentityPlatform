export const authServer = 'http://127.0.0.1:5001';
export const apiServer = 'http://127.0.0.1:5002';
export const clientId = 'demo-spa';
export const redirectUri = 'http://127.0.0.1:5173/callback';
export const scope = 'openid profile content.read';

// 中文注释: 统一管理本地存储 key，避免组件里散落字符串。
export const storageKeys = {
  tokens: 'authflowlab.tokens',
  verifier: 'authflowlab.pkce.verifier',
  state: 'authflowlab.pkce.state',
  nonce: 'authflowlab.pkce.nonce'
} as const;

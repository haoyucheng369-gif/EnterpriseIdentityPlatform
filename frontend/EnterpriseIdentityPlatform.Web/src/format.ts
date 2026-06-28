export function stringClaim(value: unknown) {
  if (Array.isArray(value)) {
    return value.join(', ');
  }

  return typeof value === 'string' || typeof value === 'number' ? String(value) : '-';
}

export function formatBody(body: string) {
  try {
    return JSON.stringify(JSON.parse(body), null, 2);
  } catch {
    return body;
  }
}

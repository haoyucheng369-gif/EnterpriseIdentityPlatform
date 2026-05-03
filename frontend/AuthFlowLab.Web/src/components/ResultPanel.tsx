import { formatBody } from '../format';
import type { CallResult } from '../types';

export function ResultPanel({ result }: { result: CallResult | null }) {
  return (
    <section className="card content-card">
      <h2>Result</h2>
      {result ? (
        <pre>{`${result.label}\nHTTP ${result.status}\n\n${formatBody(result.body)}`}</pre>
      ) : (
        <p className="empty">No API call yet.</p>
      )}
    </section>
  );
}

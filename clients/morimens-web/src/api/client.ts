import type { Awakener, DamageCalculationRequest, DamageResult } from '../types';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000';

async function get<T>(path: string): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`);
  if (!res.ok) throw new Error(`GET ${path} → ${res.status} ${res.statusText}`);
  return res.json() as Promise<T>;
}

async function post<TBody, TResult>(path: string, body: TBody): Promise<TResult> {
  const res = await fetch(`${BASE_URL}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(`POST ${path} → ${res.status}: ${text}`);
  }
  return res.json() as Promise<TResult>;
}

export const api = {
  getAwakeners: ()                              => get<Awakener[]>('/api/awakener'),
  getAwakener:  (id: number)                   => get<Awakener>(`/api/awakener/${id}`),
  calculate:    (req: DamageCalculationRequest) => post<DamageCalculationRequest, DamageResult>('/api/damage/calculate', req),
};

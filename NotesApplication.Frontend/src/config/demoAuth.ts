/** Fixed demo account for static hosting (e.g. GitHub Pages) when no API is available. */

export const DEMO_BEARER_TOKEN = 'demo-gh-pages-bearer'

export const DEMO_USER = {
  userId: 1,
  username: 'demo',
  email: 'demo@notes.local',
  token: DEMO_BEARER_TOKEN,
}

export const DEMO_CREDENTIALS = {
  email: 'demo@notes.local',
  password: 'demo',
} as const

export function isDemoToken(token: string | null | undefined): boolean {
  return !!token && token === DEMO_BEARER_TOKEN
}

/** Show demo login when explicitly enabled, or in production on *.github.io */
export function isDemoLoginEnabled(): boolean {
  if (import.meta.env.VITE_DEMO_LOGIN === 'false') return false
  if (import.meta.env.VITE_DEMO_LOGIN === 'true') return true
  if (typeof window === 'undefined') return false
  return (
    import.meta.env.PROD &&
    /\.github\.io$/i.test(window.location.hostname)
  )
}

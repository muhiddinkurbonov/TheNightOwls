import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider, useAuth } from '@/providers/AuthProvider';
import { authApi } from '@/lib/api/auth';
import type { UserDto } from '@/types/api';

// Mock Next.js router
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: vi.fn(),
    replace: vi.fn(),
    prefetch: vi.fn(),
  }),
  usePathname: () => '/',
}));

vi.mock('@/lib/api/auth', () => ({
  authApi: {
    me: vi.fn(),
    login: vi.fn(),
    register: vi.fn(),
  },
}));

// Test component that uses the auth hook
const TestComponent = () => {
  const { user, isAuthenticated, isLoading } = useAuth();

  if (isLoading) return <div>Loading...</div>;
  if (isAuthenticated && user) return <div>Authenticated as {user.username}</div>;
  return <div>Not authenticated</div>;
};

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });
  // eslint-disable-next-line react/display-name
  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>{children}</AuthProvider>
    </QueryClientProvider>
  );
};

describe('AuthProvider', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.clear();
  });

  it('should render children when not authenticated', async () => {
    vi.mocked(authApi.me).mockRejectedValue(new Error('Unauthorized'));

    const Wrapper = createWrapper();
    render(<TestComponent />, { wrapper: Wrapper });

    await waitFor(() => {
      expect(screen.getByText('Not authenticated')).toBeInTheDocument();
    });
  });

  it('should authenticate user when token exists', async () => {
    const mockUser: UserDto = {
      userId: 1,
      username: 'testuser',
      email: 'test@example.com',
      name: 'Test User',
      phoneNumber: '555-0123',
      role: 'Customer',
      createdAt: new Date(),
      isActive: true,
    };

    localStorage.setItem('token', 'fake-token');
    vi.mocked(authApi.me).mockResolvedValue(mockUser);

    const Wrapper = createWrapper();
    render(<TestComponent />, { wrapper: Wrapper });

    await waitFor(() => {
      expect(screen.getByText('Authenticated as testuser')).toBeInTheDocument();
    });

    expect(authApi.me).toHaveBeenCalled();
  });

  it('should show loading state initially', () => {
    vi.mocked(authApi.me).mockImplementation(
      () => new Promise(() => {}) // Never resolves
    );

    localStorage.setItem('token', 'fake-token');

    const Wrapper = createWrapper();
    render(<TestComponent />, { wrapper: Wrapper });

    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('should handle authentication error and clear token', async () => {
    localStorage.setItem('token', 'invalid-token');
    vi.mocked(authApi.me).mockRejectedValue(new Error('Unauthorized'));

    const Wrapper = createWrapper();
    render(<TestComponent />, { wrapper: Wrapper });

    await waitFor(() => {
      expect(screen.getByText('Not authenticated')).toBeInTheDocument();
    });

    expect(localStorage.getItem('token')).toBeNull();
  });
});

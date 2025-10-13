import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Navigation } from '@/components/Navigation';
import { AuthProvider } from '@/providers/AuthProvider';
import { authApi } from '@/lib/api/auth';

// Mock next/navigation
vi.mock('next/navigation', () => ({
  usePathname: vi.fn(() => '/'),
  useRouter: vi.fn(() => ({
    push: vi.fn(),
    replace: vi.fn(),
    refresh: vi.fn(),
  })),
}));

// Mock next/link
vi.mock('next/link', () => ({
  default: ({ children, href, className }: { children: React.ReactNode; href: string; className?: string }) => (
    <a href={href} className={className}>
      {children}
    </a>
  ),
}));

// Mock auth API
vi.mock('@/lib/api/auth', () => ({
  authApi: {
    me: vi.fn(() => Promise.reject(new Error('Not authenticated'))),
    login: vi.fn(),
    register: vi.fn(),
  },
}));

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

describe('Navigation', () => {
  it('should render the brand name', () => {
    const Wrapper = createWrapper();
    render(<Navigation />, { wrapper: Wrapper });
    expect(screen.getByText('Fadebook')).toBeInTheDocument();
  });

  it('should render all navigation links', () => {
    const Wrapper = createWrapper();
    render(<Navigation />, { wrapper: Wrapper });

    expect(screen.getByText('Home')).toBeInTheDocument();
    expect(screen.getByText('My Appointments')).toBeInTheDocument();
    expect(screen.getByText('Barbers')).toBeInTheDocument();
    expect(screen.getByText('Book Appointment')).toBeInTheDocument();
    expect(screen.getByText('Admin')).toBeInTheDocument();
  });

  it('should have correct hrefs for all links', () => {
    const Wrapper = createWrapper();
    render(<Navigation />, { wrapper: Wrapper });

    const homeLink = screen.getByText('Home').closest('a');
    const appointmentsLink = screen.getByText('My Appointments').closest('a');
    const barbersLink = screen.getByText('Barbers').closest('a');
    const bookLink = screen.getByText('Book Appointment').closest('a');
    const adminLink = screen.getByText('Admin').closest('a');

    expect(homeLink).toHaveAttribute('href', '/');
    expect(appointmentsLink).toHaveAttribute('href', '/my-appointments');
    expect(barbersLink).toHaveAttribute('href', '/barbers');
    expect(bookLink).toHaveAttribute('href', '/book');
    expect(adminLink).toHaveAttribute('href', '/admin');
  });

  it('should apply active styles to current path', async () => {
    const nextNav = await import('next/navigation');
    vi.mocked(nextNav.usePathname).mockReturnValue('/my-appointments');

    const Wrapper = createWrapper();
    render(<Navigation />, { wrapper: Wrapper });

    const appointmentsLink = screen.getByText('My Appointments').closest('a');
    expect(appointmentsLink?.className).toContain('bg-primary');
    expect(appointmentsLink?.className).toContain('text-primary-foreground');
  });

  it('should apply hover styles to non-active links', async () => {
    const nextNav = await import('next/navigation');
    vi.mocked(nextNav.usePathname).mockReturnValue('/');

    const Wrapper = createWrapper();
    render(<Navigation />, { wrapper: Wrapper });

    const appointmentsLink = screen.getByText('My Appointments').closest('a');
    expect(appointmentsLink?.className).toContain('text-muted-foreground');
    expect(appointmentsLink?.className).toContain('hover:bg-accent');
  });
});

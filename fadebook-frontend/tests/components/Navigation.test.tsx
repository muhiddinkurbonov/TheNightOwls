import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { Navigation } from '@/components/Navigation';

// Mock next/navigation
vi.mock('next/navigation', () => ({
  usePathname: vi.fn(() => '/'),
}));

// Mock next/link
vi.mock('next/link', () => ({
  default: ({ children, href, className }: { children: React.ReactNode; href: string; className?: string }) => (
    <a href={href} className={className}>
      {children}
    </a>
  ),
}));

describe('Navigation', () => {
  it('should render the brand name', () => {
    render(<Navigation />);
    expect(screen.getByText('Night Owls Barbershop')).toBeInTheDocument();
  });

  it('should render all navigation links', () => {
    render(<Navigation />);

    expect(screen.getByText('Home')).toBeInTheDocument();
    expect(screen.getByText('Appointments')).toBeInTheDocument();
    expect(screen.getByText('Barbers')).toBeInTheDocument();
    expect(screen.getByText('Book Appointment')).toBeInTheDocument();
  });

  it('should have correct hrefs for all links', () => {
    render(<Navigation />);

    const homeLink = screen.getByText('Home').closest('a');
    const appointmentsLink = screen.getByText('Appointments').closest('a');
    const barbersLink = screen.getByText('Barbers').closest('a');
    const bookLink = screen.getByText('Book Appointment').closest('a');

    expect(homeLink).toHaveAttribute('href', '/');
    expect(appointmentsLink).toHaveAttribute('href', '/appointments');
    expect(barbersLink).toHaveAttribute('href', '/barbers');
    expect(bookLink).toHaveAttribute('href', '/book');
  });

  it('should apply active styles to current path', async () => {
    const nextNav = await import('next/navigation');
    vi.mocked(nextNav.usePathname).mockReturnValue('/appointments');

    render(<Navigation />);

    const appointmentsLink = screen.getByText('Appointments').closest('a');
    expect(appointmentsLink?.className).toContain('bg-primary');
    expect(appointmentsLink?.className).toContain('text-primary-foreground');
  });

  it('should apply hover styles to non-active links', async () => {
    const nextNav = await import('next/navigation');
    vi.mocked(nextNav.usePathname).mockReturnValue('/');

    render(<Navigation />);

    const appointmentsLink = screen.getByText('Appointments').closest('a');
    expect(appointmentsLink?.className).toContain('text-muted-foreground');
    expect(appointmentsLink?.className).toContain('hover:bg-accent');
  });
});

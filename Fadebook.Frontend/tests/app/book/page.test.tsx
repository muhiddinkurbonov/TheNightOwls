import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import BookAppointmentPage from '@/app/book/page';
import { useAuth } from '@/providers/AuthProvider';
import { workHoursApi } from '@/lib/api';

// Mock Next.js router
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

// Mock AuthProvider
vi.mock('@/providers/AuthProvider', () => ({
  useAuth: vi.fn(),
}));

// Mock API hooks
vi.mock('@/hooks/useCustomers', () => ({
  useServices: () => ({
    data: [
      { serviceId: 1, serviceName: 'Haircut', servicePrice: 20 },
      { serviceId: 2, serviceName: 'Beard Trim', servicePrice: 15 },
    ],
    isLoading: false,
  }),
  useBarbersByService: () => ({
    data: [
      { barberId: 1, name: 'John Barber', specialty: 'Haircuts' },
    ],
    isLoading: false,
  }),
  useAllCustomers: () => ({
    data: [],
    isLoading: false,
  }),
}));

vi.mock('@/hooks/useAppointments', () => ({
  useCreateAppointment: () => ({
    mutateAsync: vi.fn(),
    isPending: false,
  }),
}));

vi.mock('@/lib/api', () => ({
  workHoursApi: {
    getAvailableTimeSlots: vi.fn(),
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
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
};

describe('BookAppointmentPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should redirect to signin if not authenticated', () => {
    vi.mocked(useAuth).mockReturnValue({
      user: null,
      isAuthenticated: false,
      isLoading: false,
      login: vi.fn(),
      register: vi.fn(),
      logout: vi.fn(),
    });

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <BookAppointmentPage />
      </Wrapper>
    );

    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  it('should render booking form for authenticated customer', async () => {
    vi.mocked(useAuth).mockReturnValue({
      user: {
        userId: 1,
        username: 'testcustomer',
        email: 'test@example.com',
        name: 'Test Customer',
        role: 'Customer',
        phoneNumber: '555-0123',
        createdAt: new Date(),
        isActive: true,
      },
      isAuthenticated: true,
      isLoading: false,
      login: vi.fn(),
      register: vi.fn(),
      logout: vi.fn(),
    });

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <BookAppointmentPage />
      </Wrapper>
    );

    await waitFor(() => {
      expect(screen.getByText('Book an Appointment')).toBeInTheDocument();
    });

    expect(screen.getByText('Appointment Details')).toBeInTheDocument();
  });

  it('should load available time slots when barber and date are selected', async () => {
    vi.mocked(useAuth).mockReturnValue({
      user: {
        userId: 1,
        username: 'testcustomer',
        email: 'test@example.com',
        name: 'Test Customer',
        role: 'Customer',
        phoneNumber: '555-0123',
        createdAt: new Date(),
        isActive: true,
      },
      isAuthenticated: true,
      isLoading: false,
      login: vi.fn(),
      register: vi.fn(),
      logout: vi.fn(),
    });

    const mockSlots = [
      '2025-10-15T09:00:00',
      '2025-10-15T09:30:00',
      '2025-10-15T10:00:00',
    ];

    vi.mocked(workHoursApi.getAvailableTimeSlots).mockResolvedValue(mockSlots);

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <BookAppointmentPage />
      </Wrapper>
    );

    await waitFor(() => {
      expect(screen.getByText('Book an Appointment')).toBeInTheDocument();
    });
  });

  it('should show customer selector for barbers', async () => {
    vi.mocked(useAuth).mockReturnValue({
      user: {
        userId: 2,
        username: 'testbarber',
        email: 'barber@example.com',
        name: 'Test Barber',
        role: 'Barber',
        phoneNumber: '555-0124',
        createdAt: new Date(),
        isActive: true,
        barberId: 1,
      },
      isAuthenticated: true,
      isLoading: false,
      login: vi.fn(),
      register: vi.fn(),
      logout: vi.fn(),
    });

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <BookAppointmentPage />
      </Wrapper>
    );

    await waitFor(() => {
      expect(screen.getByText('Customer')).toBeInTheDocument();
    });
  });
});

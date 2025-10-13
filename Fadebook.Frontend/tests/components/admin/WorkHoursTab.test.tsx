import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { WorkHoursTab } from '@/components/admin/WorkHoursTab';
import { workHoursApi, barbersApi } from '@/lib/api';
import type { BarberWorkHoursDto } from '@/types/api';

vi.mock('@/lib/api', () => ({
  workHoursApi: {
    getAll: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
  },
  barbersApi: {
    getAll: vi.fn(),
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

describe('WorkHoursTab', () => {
  const mockBarbers = [
    { barberId: 1, name: 'John Barber', username: 'john.barber', specialty: 'Haircuts', contactInfo: '555-0101' },
    { barberId: 2, name: 'Jane Barber', username: 'jane.barber', specialty: 'Styling', contactInfo: '555-0102' },
  ];

  const mockWorkHours: BarberWorkHoursDto[] = [
    {
      workHourId: 1,
      barberId: 1,
      barberName: 'John Barber',
      dayOfWeek: 1,
      dayOfWeekName: 'Monday',
      startTime: '09:00',
      endTime: '17:00',
      isActive: true,
    },
    {
      workHourId: 2,
      barberId: 1,
      barberName: 'John Barber',
      dayOfWeek: 2,
      dayOfWeekName: 'Tuesday',
      startTime: '09:00',
      endTime: '17:00',
      isActive: true,
    },
  ];

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(barbersApi.getAll).mockResolvedValue(mockBarbers);
  });

  it('should display loading state initially', () => {
    vi.mocked(workHoursApi.getAll).mockImplementation(
      () => new Promise(() => {}) // Never resolves
    );

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <WorkHoursTab />
      </Wrapper>
    );

    expect(screen.getByText('Loading work hours...')).toBeInTheDocument();
  });

  it('should display work hours table when data is loaded', async () => {
    vi.mocked(workHoursApi.getAll).mockResolvedValue(mockWorkHours);

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <WorkHoursTab />
      </Wrapper>
    );

    await waitFor(() => {
      expect(screen.getByText('John Barber')).toBeInTheDocument();
    });

    expect(screen.getByText('Monday')).toBeInTheDocument();
    expect(screen.getByText('Tuesday')).toBeInTheDocument();
    expect(screen.getByText('09:00')).toBeInTheDocument();
    expect(screen.getByText('17:00')).toBeInTheDocument();
  });

  it('should display empty state when no work hours exist', async () => {
    vi.mocked(workHoursApi.getAll).mockResolvedValue([]);

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <WorkHoursTab />
      </Wrapper>
    );

    await waitFor(() => {
      expect(screen.getByText('No work hours configured')).toBeInTheDocument();
    });
  });

  it('should display error state on load failure', async () => {
    vi.mocked(workHoursApi.getAll).mockRejectedValue(new Error('Failed to load'));

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <WorkHoursTab />
      </Wrapper>
    );

    await waitFor(() => {
      expect(screen.getByText('Error loading work hours')).toBeInTheDocument();
    });
  });

  it('should open add dialog when clicking Add Work Hours button', async () => {
    vi.mocked(workHoursApi.getAll).mockResolvedValue(mockWorkHours);

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <WorkHoursTab />
      </Wrapper>
    );

    await waitFor(() => {
      expect(screen.getByText('Add Work Hours')).toBeInTheDocument();
    });

    const addButton = screen.getByText('Add Work Hours');
    fireEvent.click(addButton);

    await waitFor(() => {
      expect(screen.getByText('Add New Work Hours')).toBeInTheDocument();
    });
  });

  it('should display Active badge for active work hours', async () => {
    vi.mocked(workHoursApi.getAll).mockResolvedValue(mockWorkHours);

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <WorkHoursTab />
      </Wrapper>
    );

    await waitFor(() => {
      const activeBadges = screen.getAllByText('Active');
      expect(activeBadges.length).toBeGreaterThan(0);
    });
  });

  it('should display Inactive badge for inactive work hours', async () => {
    const inactiveWorkHours: BarberWorkHoursDto[] = [
      {
        workHourId: 3,
        barberId: 1,
        barberName: 'John Barber',
        dayOfWeek: 3,
        dayOfWeekName: 'Wednesday',
        startTime: '09:00',
        endTime: '17:00',
        isActive: false,
      },
    ];

    vi.mocked(workHoursApi.getAll).mockResolvedValue(inactiveWorkHours);

    const Wrapper = createWrapper();
    render(
      <Wrapper>
        <WorkHoursTab />
      </Wrapper>
    );

    await waitFor(() => {
      expect(screen.getByText('Inactive')).toBeInTheDocument();
    });
  });
});

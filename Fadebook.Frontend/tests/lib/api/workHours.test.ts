import { describe, it, expect, vi, beforeEach } from 'vitest';
import { workHoursApi } from '@/lib/api/workHours';
import type { BarberWorkHoursDto, CreateBarberWorkHoursDto } from '@/types/api';

const mockAxios = {
  get: vi.fn(),
  post: vi.fn(),
  put: vi.fn(),
  delete: vi.fn(),
};

vi.mock('@/lib/axios', () => ({
  default: mockAxios,
}));

describe('workHoursApi', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('getAll', () => {
    it('should fetch all work hours', async () => {
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
      ];

      mockAxios.get.mockResolvedValue({ data: mockWorkHours });

      const result = await workHoursApi.getAll();

      expect(mockAxios.get).toHaveBeenCalledWith('/api/BarberWorkHours');
      expect(result).toEqual(mockWorkHours);
    });
  });

  describe('getById', () => {
    it('should fetch work hours by id', async () => {
      const mockWorkHours: BarberWorkHoursDto = {
        workHourId: 1,
        barberId: 1,
        barberName: 'John Barber',
        dayOfWeek: 1,
        dayOfWeekName: 'Monday',
        startTime: '09:00',
        endTime: '17:00',
        isActive: true,
      };

      mockAxios.get.mockResolvedValue({ data: mockWorkHours });

      const result = await workHoursApi.getById(1);

      expect(mockAxios.get).toHaveBeenCalledWith('/api/BarberWorkHours/1');
      expect(result).toEqual(mockWorkHours);
    });
  });

  describe('getByBarberId', () => {
    it('should fetch work hours by barber id', async () => {
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
      ];

      mockAxios.get.mockResolvedValue({ data: mockWorkHours });

      const result = await workHoursApi.getByBarberId(1);

      expect(mockAxios.get).toHaveBeenCalledWith('/api/BarberWorkHours/barber/1');
      expect(result).toEqual(mockWorkHours);
    });
  });

  describe('getAvailableTimeSlots', () => {
    it('should fetch available time slots', async () => {
      const mockSlots = [
        '2025-10-15T09:00:00',
        '2025-10-15T09:30:00',
        '2025-10-15T10:00:00',
      ];

      mockAxios.get.mockResolvedValue({ data: mockSlots });

      const result = await workHoursApi.getAvailableTimeSlots(1, '2025-10-15', 30);

      expect(mockAxios.get).toHaveBeenCalledWith('/api/BarberWorkHours/barber/1/slots', {
        params: { date: '2025-10-15', durationMinutes: 30 },
      });
      expect(result).toEqual(mockSlots);
    });

    it('should use default duration of 30 minutes', async () => {
      const mockSlots = ['2025-10-15T09:00:00'];

      mockAxios.get.mockResolvedValue({ data: mockSlots });

      await workHoursApi.getAvailableTimeSlots(1, '2025-10-15');

      expect(mockAxios.get).toHaveBeenCalledWith('/api/BarberWorkHours/barber/1/slots', {
        params: { date: '2025-10-15', durationMinutes: 30 },
      });
    });
  });

  describe('create', () => {
    it('should create new work hours', async () => {
      const newWorkHours: CreateBarberWorkHoursDto = {
        barberId: 1,
        dayOfWeek: 1,
        startTime: '09:00',
        endTime: '17:00',
        isActive: true,
      };

      const mockResponse: BarberWorkHoursDto = {
        workHourId: 1,
        barberId: 1,
        barberName: 'John Barber',
        dayOfWeek: 1,
        dayOfWeekName: 'Monday',
        startTime: '09:00',
        endTime: '17:00',
        isActive: true,
      };

      mockAxios.post.mockResolvedValue({ data: mockResponse });

      const result = await workHoursApi.create(newWorkHours);

      expect(mockAxios.post).toHaveBeenCalledWith('/api/BarberWorkHours', newWorkHours);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('update', () => {
    it('should update existing work hours', async () => {
      const updatedWorkHours: CreateBarberWorkHoursDto = {
        barberId: 1,
        dayOfWeek: 1,
        startTime: '08:00',
        endTime: '16:00',
        isActive: true,
      };

      const mockResponse: BarberWorkHoursDto = {
        workHourId: 1,
        barberId: 1,
        barberName: 'John Barber',
        dayOfWeek: 1,
        dayOfWeekName: 'Monday',
        startTime: '08:00',
        endTime: '16:00',
        isActive: true,
      };

      mockAxios.put.mockResolvedValue({ data: mockResponse });

      const result = await workHoursApi.update(1, updatedWorkHours);

      expect(mockAxios.put).toHaveBeenCalledWith('/api/BarberWorkHours/1', updatedWorkHours);
      expect(result).toEqual(mockResponse);
    });
  });

  describe('delete', () => {
    it('should delete work hours', async () => {
      mockAxios.delete.mockResolvedValue({ data: undefined });

      await workHoursApi.delete(1);

      expect(mockAxios.delete).toHaveBeenCalledWith('/api/BarberWorkHours/1');
    });
  });
});

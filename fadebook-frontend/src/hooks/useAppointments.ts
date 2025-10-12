import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { appointmentsApi } from '@/lib/api';
import type { CreateAppointmentDto } from '@/types/api';

export const useAppointment = (id: number) => {
  return useQuery({
    queryKey: ['appointment', id],
    queryFn: () => appointmentsApi.getById(id),
    enabled: !!id,
  });
};

export const useAppointmentsByDate = (date: string) => {
  return useQuery({
    queryKey: ['appointments', 'by-date', date],
    queryFn: () => appointmentsApi.getByDate(date),
    enabled: !!date,
    refetchInterval: 10000, // Auto-refetch every 10 seconds
    refetchIntervalInBackground: false, // Only refetch when tab is active
  });
};

export const useAppointmentsByUsername = (username: string) => {
  return useQuery({
    queryKey: ['appointments', 'by-username', username],
    queryFn: () => appointmentsApi.getByUsername(username),
    enabled: !!username,
    refetchInterval: 10000, // Auto-refetch every 10 seconds
    refetchIntervalInBackground: false, // Only refetch when tab is active
  });
};

export const useAppointmentsByBarberId = (barberId: number) => {
  return useQuery({
    queryKey: ['appointments', 'by-barber', barberId],
    queryFn: () => appointmentsApi.getByBarberId(barberId),
    enabled: !!barberId,
    refetchInterval: 10000, // Auto-refetch every 10 seconds
    refetchIntervalInBackground: false, // Only refetch when tab is active
  });
};

export const useCreateAppointment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (appointment: CreateAppointmentDto) => appointmentsApi.create(appointment),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['appointments'] });
    },
  });
};

export const useUpdateAppointment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, appointment }: { id: number; appointment: CreateAppointmentDto }) =>
      appointmentsApi.update(id, appointment),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['appointment', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['appointments'] });
    },
  });
};

export const useDeleteAppointment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => appointmentsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['appointments'] });
    },
  });
};

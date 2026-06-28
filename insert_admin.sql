-- Insert de usuário administrador para o sistema FinanSee
-- Credenciais:
-- Email: admin@finansee.com
-- Senha: admin123
-- Hash BCrypt gerado para a senha "admin123" com custo 10

-- Nota: Ajuste o nome da tabela e coluna conforme sua configuração:
-- - Se usar o schema SQL original: tabela "usuarios" e coluna "senha"
-- - Se usar o mapeamento EF Core: tabela "usuario" e coluna "SenhaHash" ou "senha_hash"

-- Para o mapeamento EF Core (usuario)
INSERT INTO usuario (nome, email, "SenhaHash", perfil, ativo)
VALUES (
  'admin',
  'admin@finansee.com',
  '$2a$10$N9qo8uLOickgx2ZMRZoMye1tLqH.IbIjJqJ5z8tNKLLlGPFqYFZWi',
  'admin',
  true
);

-- OU, se a coluna usar snake_case:
-- INSERT INTO usuario (nome, email, senha_hash, perfil, ativo)
-- VALUES (
--   'Administrador',
--   'admin@finansee.com',
--   '$2a$10$N9qo8uLOickgx2ZMRZoMye1tLqH.IbIjJqJ5z8tNKLLlGPFqYFZWi',
--   'admin',
--   true
-- );

-- OU, se usar o schema original (usuarios):
-- INSERT INTO usuarios (nome, email, senha, perfil, ativo)
-- VALUES (
--   'Administrador',
--   'admin@finansee.com',
--   '$2a$10$N9qo8uLOickgx2ZMRZoMye1tLqH.IbIjJqJ5z8tNKLLlGPFqYFZWi',
--   'admin',
--   true
-- );

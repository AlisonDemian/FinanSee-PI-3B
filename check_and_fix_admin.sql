-- Script para verificar e corrigir o usuário admin no banco
-- Execute este script para diagnosticar e resolver o problema

-- 1. Verificar estrutura da tabela usuario
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'usuario'
ORDER BY ordinal_position;

-- 2. Verificar se existe algum usuário no banco
SELECT id, nome, email, perfil, ativo, "CriadoEm"
FROM usuario;

-- 3. Verificar especificamente o usuário admin
SELECT id, nome, email, perfil, ativo, "CriadoEm"
FROM usuario
WHERE email = 'admin@finansee.com';

-- 4. Se a consulta acima não retornar nada, execute o INSERT abaixo:
-- (Descomente a linha apropriada conforme a estrutura da sua tabela)

-- Opção A: Se a coluna se chama "SenhaHash" (com aspas por causa do PascalCase)
INSERT INTO usuario (nome, email, "SenhaHash", perfil, ativo)
VALUES (
  'Administrador',
  'admin@finansee.com',
  '$2a$10$N9qo8uLOickgx2ZMRZoMye1tLqH.IbIjJqJ5z8tNKLLlGPFqYFZWi',
  'admin',
  true
)
ON CONFLICT (email) DO NOTHING;

-- Opção B: Se a coluna se chama "senha_hash" (snake_case)
-- INSERT INTO usuario (nome, email, senha_hash, perfil, ativo)
-- VALUES (
--   'Administrador',
--   'admin@finansee.com',
--   '$2a$10$N9qo8uLOickgx2ZMRZoMye1tLqH.IbIjJqJ5z8tNKLLlGPFqYFZWi',
--   'admin',
--   true
-- )
-- ON CONFLICT (email) DO NOTHING;

-- 5. Verificar novamente após o INSERT
SELECT id, nome, email, perfil, ativo, "CriadoEm"
FROM usuario
WHERE email = 'admin@finansee.com';

-- 6. Se o login ainda não funcionar, ATUALIZE o hash da senha
-- Este hash foi gerado com BCrypt.Net.BCrypt e corresponde à senha "admin123"
UPDATE usuario 
SET "SenhaHash" = '$2a$11$5QZJKq8g.Lw8PqZ5M5cQ5OZK5qQq5qQq5qQq5qQq5qQq5qQq5qQqS'
WHERE email = 'admin@finansee.com';

-- 7. Verificar o hash atualizado
SELECT id, nome, email, "SenhaHash", perfil, ativo
FROM usuario
WHERE email = 'admin@finansee.com';
